using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class Interfaz : MonoBehaviour
    {
        [SerializeField]
        EventManager eventsManager;

        static public Interfaz instance;

        public List<TextCompleto> textC = new List<TextCompleto>();

        public Animation interactButton;

        [Header("Dialogo")]

        [SerializeField] Image dialogoImage;
        TextCompleto dialogoText;
        TextCompleto objectiveText;

        float   widthDiag;
        float   heightDiag;

        [Header("Vida"), SerializeField]
        HealthUI healthUI;

        [Header("Energia"), SerializeField]
        EnergyUI energyUI;

        [Header("Cooldowns"), SerializeField]
        CooldownUI[] basicCooldowns,katasCooldowns,abilitiesCooldowns;

        List<string> _objectivesToShow = new List<string>();

        string objectiveToShow => "Objetivos:\n" + string.Join('\n', _objectivesToShow);

        public TextCompleto this[string name]
        {
            get
            {
                foreach (var item in textC)
                {
                    if (item.name == name)
                        return item;
                }

                return null;
            }
        }

        static public TextCompleto SearchTitle(string name)
        {
            return instance[name];
        }

        public void Dialogo(string str)
        {
            dialogoText.AddMsg(str);
        }

        public void CompleteAllObjective()
        {
            for (int i = 0; i < _objectivesToShow.Count; i++)
            {
                _objectivesToShow[i] = _objectivesToShow[i].RichText("s");
            }
           
            objectiveText.CompleteEffect();

            objectiveText.ShowMsg(objectiveToShow);
        }

        public void CompleteObjective(int index)
        {
            _objectivesToShow[index] = _objectivesToShow[index].RichText("s");
            objectiveText.ShowMsg(objectiveToShow);

            objectiveText.CompleteEffect();
        }

        public void ModifyObjective(int index, string str)
        {
            _objectivesToShow[index] = str;
            objectiveText.WowEffect();
            objectiveText.ShowMsg(objectiveToShow);
        }

        public void AddObjective(string str)
        {
            _objectivesToShow.Add(str);
            objectiveText.WowEffect();
            objectiveText.ShowMsg(objectiveToShow);
            //return _objectivesToShow.Count - 1;
        }

        public void ProvisionalObjective(string str)
        {
            objectiveText.ClearMsg();
            objectiveText.WowEffect();
            objectiveText.ShowMsg("Objetivos:\n" + str);
        }

        public void ClearProvisionalObjective()
        {
            objectiveText.ClearMsg();
            objectiveText.WowEffect();
            objectiveText.ShowMsg(objectiveToShow);
        }

        public void ClearObjective()
        {
            _objectivesToShow.Clear();
            objectiveText.ClearMsg();
        }

        public void PopText(Entity entity, string text, Vector2? dir = null)
        {
            //instance["Danio"].AddMsg($"{text} ► {entity.name.Replace("(Clone)", "")}");

            PoolManager.SpawnPoolObject(Vector2Int.up * 2, out TextPop textDamage);

            if(dir == null)
            {
                textDamage.SetText(entity.transform, text);
            }
            else
            {
                textDamage.SetText(entity.transform, text, dir, true);
            }
        }

        void CharacterSelected(Character ch)
        {
            energyUI.EnergyBarUpdate((ch.caster.positiveEnergy / ch.caster.MaxEnergy, 0,ch.caster.positiveEnergy));
            healthUI.healthBarUpdate(ch.health);
        }

        IEnumerator MyCoroutine(System.Action<bool> end, System.Action<string> msg)
        {
            msg("Interfaz");
            end(true);
            yield return null;
            eventsManager.events.SearchOrCreate<SingleEvent<Character>>("Character").delegato += CharacterSelected;

            eventsManager.events.SearchOrCreate<SingleEvent<Health>>(LifeType.all).delegato += healthUI.healthBarUpdate;

            var aux = eventsManager.events.SearchOrCreate<TripleEvent<(float, float, float), float, float>>("EnergyUpdate");

            for (int i = 0; i < basicCooldowns.Length + katasCooldowns.Length + abilitiesCooldowns.Length; i++)
            {
                SingleEvent<(Ability, ItemBase)> eventCooldown = eventsManager.events.SearchOrCreate<SingleEvent<(Ability, ItemBase)>>("abilityUI" + i);

                if (i< basicCooldowns.Length)
                {
                    var index = i;
                    eventCooldown.delegato += ((Ability, ItemBase) param) =>
                    {
                        basicCooldowns[index].SetCooldown(param.Item1, param.Item2);
                    };
                }
                else if(i < basicCooldowns.Length + katasCooldowns.Length)
                {
                    var index = i;
                    eventCooldown.delegato += ((Ability, ItemBase) param) =>
                    {
                        katasCooldowns[index- basicCooldowns.Length].SetCooldown(param.Item1, param.Item2);
                    };
                }
                else
                {
                    var index = i;
                    eventCooldown.delegato += ((Ability, ItemBase) param) =>
                    {
                        abilitiesCooldowns[index - basicCooldowns.Length - katasCooldowns.Length].SetCooldown(param.Item1, param.Item2);
                    };
                }
            }

            aux.delegato += energyUI.EnergyBarUpdate;
            aux.secondDelegato += energyUI.EnergyLeft;
            aux.thirdDelegato += energyUI.EnergyRight;
        }

        void LateUpdate()
        {
            if (dialogoText.text != "" || dialogoImage.rectTransform.rect.width > 0)
            {
                float aux1 = 0;
                float aux2 = dialogoImage.rectTransform.rect.width;

                if (dialogoText.text != "")
                {
                    aux1 = widthDiag;
                    aux2 = widthDiag - dialogoImage.rectTransform.rect.width;
                    dialogoImage.enabled = true;
                }

                dialogoImage.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(dialogoImage.rectTransform.rect.width, aux1, Time.deltaTime * (widthDiag / aux2)), heightDiag);
                //dialogoText.texto.rectTransform.sizeDelta = new Vector2(dialogoImage.rectTransform.sizeDelta.x - 100, dialogoText.texto.rectTransform.sizeDelta.y);

                if (Mathf.Approximately(dialogoImage.rectTransform.rect.width, 0))
                {
                    dialogoImage.enabled = false;
                    enabled = false;
                }
            }
        }

        void Start()
        {
            SearchTitle("Titulo secundario").ClearMsg();

            dialogoText.on += () => enabled = true;
            dialogoText.ClearMsg();

            widthDiag = dialogoImage.rectTransform.rect.width;
            heightDiag=dialogoImage.rectTransform.rect.height;

            dialogoImage.rectTransform.sizeDelta = Vector2.zero;
       
            LoadSystem.AddPostLoadCorutine(MyCoroutine, 10);
        }

        private void Awake()
        {
            instance = this;

            foreach (var item in textC)
            {
                item.Init();
            }

            dialogoText = SearchTitle("Subtitulo");

            objectiveText = SearchTitle("Objetivos");
        }
    }

    [System.Serializable]
    public class ImageWidth
    {
        public Image image;
        public float widthMax = 100;

        public float FillAmount
        {
            set
            {
                var sizeDelta = image.rectTransform.sizeDelta;

                _fillAmount = Mathf.Clamp(value, 0, 1);

                sizeDelta.x = _fillAmount * widthMax;

                image.rectTransform.sizeDelta = sizeDelta;
            }

            get => _fillAmount;
        }

        float _fillAmount;
    }
}