using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Reflection;


namespace CustomEulerEditor
{

    [CustomEditor(typeof(AnimationInfo))]
    public class AnimationsInfoEditor : InheterenceEditorOrder
    {
        AnimationInfo info;

        AnimationInfo.Data data = null;

        Editor editorAnim;

        FloatField currentTimeProperty;

        TextField nameEvent;

        public override VisualElement CreateInspectorGUI()
        {
            info = target as AnimationInfo;

            var container = base.CreateInspectorGUI();


            currentTimeProperty = new FloatField("Current Time");

            currentTimeProperty.SetEnabled(false);

            currentTimeProperty.style.width = new Length(25, LengthUnit.Percent);

            //currentTimeProperty.style.marginRight = new Length(5, LengthUnit.Percent);


            nameEvent = new TextField("Name of event");

            nameEvent.style.width = new Length(33, LengthUnit.Percent);

            //nameEvent.style.marginRight = new Length(5, LengthUnit.Percent);


            var buttonEvent = new Button(() =>
            {
                data?.events.CreateOrSave(nameEvent.value, currentTimeProperty.value);
            });

            buttonEvent.text = "AddEvent";

            //buttonEvent.style.width = new Length(25, LengthUnit.Percent);

            //buttonEvent.style.marginRight = new Length(5, LengthUnit.Percent);


            var subContainer = new VisualElement();

            subContainer.style.marginTop = 15;

            subContainer.style.flexDirection = FlexDirection.Row;

            subContainer.style.justifyContent = Justify.SpaceBetween;

            subContainer.Add(currentTimeProperty);

            subContainer.Add(nameEvent);

            subContainer.Add(buttonEvent);

            container.Add(subContainer);



            onSelectedItem += AnimationsInfoEditor_onSelectedItem;


            return container;
        }

        private void AnimationsInfoEditor_onSelectedItem(object obj)
        {
            Debug.Log(obj);

            if (obj is Internal.Pictionary<string, AnimationInfo.Data> data)
            {
                if (editorAnim == null || (this.data != data.value))
                {
                    this.data = data.value;

                    if (this.data?.animationClip != null)
                        editorAnim = CreateEditor(this.data.animationClip);
                }
            }
        }

        public override bool HasPreviewGUI()
        {
            return editorAnim?.HasPreviewGUI() ?? false;
        }

        public override void OnPreviewSettings()
        {
            editorAnim?.OnPreviewSettings();
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            FixPreviewEditorForAnimation(editorAnim);
            editorAnim?.OnInteractivePreviewGUI(r, background);
        }


        //https://discussions.unity.com/t/unity-custom-editor-window-with-animation-clip-preview/857336

        private static FieldInfo _cachedAvatarPreviewFieldInfo;
        private static FieldInfo _cachedTimeControlFieldInfo;
        private static FieldInfo _cachedStopTimeFieldInfo;
        private static FieldInfo _cachedCurrentTimeFieldInfo;

        private void FixPreviewEditorForAnimation(Editor editor)
        {
            if (data?.animationClip == null)
                return;

            if (_cachedAvatarPreviewFieldInfo != null && _cachedTimeControlFieldInfo != null && _cachedStopTimeFieldInfo != null)
            {
                var value = _cachedAvatarPreviewFieldInfo.GetValue(editor);
                var subValue = _cachedTimeControlFieldInfo.GetValue(value);
                _cachedStopTimeFieldInfo.SetValue(subValue, data.animationClip.length);

                currentTimeProperty.value = /*data.offsetTime +*/ (float)_cachedCurrentTimeFieldInfo.GetValue(subValue);
            }
            else
            {
                _cachedAvatarPreviewFieldInfo ??= editor.GetType().GetField("m_AvatarPreview", BindingFlags.NonPublic | BindingFlags.Instance);

                if (_cachedAvatarPreviewFieldInfo == null) return;

                var value = _cachedAvatarPreviewFieldInfo.GetValue(editor);

                if (value == null) return;

                _cachedTimeControlFieldInfo ??= value.GetType().GetField("timeControl", BindingFlags.Public | BindingFlags.Instance);

                if (_cachedTimeControlFieldInfo == null) return;

                var subValue = _cachedTimeControlFieldInfo.GetValue(value);

                if (subValue == null) return;

                _cachedStopTimeFieldInfo ??= subValue.GetType().GetField("stopTime", BindingFlags.Public | BindingFlags.Instance);

                _cachedCurrentTimeFieldInfo ??= subValue.GetType().GetField("currentTime", BindingFlags.Public | BindingFlags.Instance);

                if (_cachedStopTimeFieldInfo == null) return;

                _cachedStopTimeFieldInfo.SetValue(subValue, data.animationClip.length);
            }
        }
    }
}