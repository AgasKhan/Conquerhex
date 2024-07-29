using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UI
{
    public class TextCompleto : MonoBehaviour, Init
    {
        [System.Serializable]
        public class GradientOverTime
        {
            [SerializeField]
            Gradient grad;

            [SerializeField]
            float timeIn;

            [SerializeField]
            float timeGradient;

            [SerializeField]
            float timeOut;

            [SerializeReference]
            TimedCompleteAction timerGradient;

            //[SerializeReference]
            TimedCompleteAction timerIn;

            //[SerializeReference]
            TimedCompleteAction timerOut;

            Color originalColor;

            public void Execute()
            {
                timerIn.Reset();
            }

            public void Cancel()
            {
                timerGradient.Stop();
                timerIn.Stop();
                timerOut.Reset();
            }

            public void Init(System.Func<Color?, Color> actionToApplyGrad)
            {
                originalColor = actionToApplyGrad(null);

                timerIn = TimersManager.Create(originalColor, grad.Evaluate(0), timeIn, Color.Lerp, (color) => actionToApplyGrad(color));

                timerOut = TimersManager.Create(grad.Evaluate(1), originalColor, timeOut, Color.Lerp, (color) => actionToApplyGrad(color));

                timerGradient = TimersManager.Create(timeGradient, () => actionToApplyGrad(grad.Evaluate(timerGradient.InversePercentage())), () => timerOut.Reset());

                timerIn.AddToEnd(() => timerGradient.Reset());

                timerIn.Stop();
                timerGradient.Stop();
                timerOut.Stop();
            }
        }

        [Header("Seteo")]

        [SerializeField] TextMeshProUGUI texto;
        [field: SerializeField] [field: TextArea(3, 6)] public string final { get; private set; }

        [SerializeField] float tiempoEntreLetras;

        [SerializeField] float tiempoEntrePaginas;

        [SerializeField] float tiempoParaDesaparecer = 3;

        [SerializeField]
        FadeOnOff fadeMenu;

        [SerializeField]
        bool useAutomaticGradients;

        [SerializeField]
        GradientOverTime wow;

        [SerializeField]
        GradientOverTime cancel;

        [SerializeField]
        GradientOverTime complete;

        [Header("Para debug")]
        [SerializeReference]
        Timer timerToHide;
        [SerializeReference]
        Timer letras;
        [SerializeReference]
        Timer entrePaginas;

        static char[] saltos = new char[] { '\n', '.', '!', '?' };

        public event System.Action on;

        public event System.Action off;
        public float velocityMultiply => letras.Multiply;

        public string text => texto.text;

        private void FadeMenu_alphas(float obj)
        {
            texto.color = texto.color.ChangeAlphaCopy(obj);
        }

        public void AcelerateMsg(float velocity)
        {
            letras.SetMultiply(velocity);

            if (!entrePaginas.Chck)
                entrePaginas.SetInitCurrent(0);

            if (final == string.Empty)
                timerToHide?.SetInitCurrent(0);
        }

        public void WowEffect()
        {
            wow.Execute();
        }

        public void CancelEffect()
        {
            cancel.Execute();
        }

        public void CompleteEffect()
        {
            complete.Execute();
        }

        /// <summary>
        /// Borra el texto del cuadro del mensaje
        /// </summary>
        /// <param name="msg"></param>
        public void ClearMsg()
        {
            letras.Stop();
            final = "";
            texto.text = "";
        }

        /// <summary>
        /// Muestra un mensaje directamente en el campo, no checkea su largo
        /// </summary>
        /// <param name="msg"></param>
        public void ShowMsg(string msg)
        {
            final = msg;
            texto.text = msg;

            if (timerToHide?.Chck ?? true)
                On();
            else
                timerToHide?.Reset();
        }

        /// <summary>
        /// Aniade un mensaje que se mostrara de forma ordenada y escrito letra por letra
        /// </summary>
        /// <param name="msg"></param>
        public void AddMsg(string msg)
        {
            if (final == "")
            {
                letras.SetMultiply(1);
                msg = texto.text + msg;
            }

            final += msg + "\n";

            On();
        }

        void On()
        {
            fadeMenu.end -= FadeMenu_end;
            SetFade(1);
            timerToHide?.Reset();
            letras.Start();
            on?.Invoke();
        }

        void SetFade(float end)
        {
            fadeMenu.SetFade(texto.color.a, end);
        }

        bool TextOverFlowing()
        {
            Vector2 rectSize = texto.rectTransform.sizeDelta;
            Vector2 preferedSize = texto.GetPreferredValues(texto.text);

            return rectSize.x < preferedSize.x && rectSize.y < preferedSize.y;
        }
        
        void EntrePaginasFixed()
        {
            if (!texto.isTextOverflowing)
            {
                entrePaginas.SetInitCurrent(0);
            }
        }

        void EntrePaginas()
        {
            var lastIndex = texto.text.LastIndexOfAny(saltos);

            if (lastIndex >= 0)
                final = final.Replace(texto.text.Substring(0, lastIndex + 1), "").Trim();
            //else
            //  final = final.Replace(texto.text, "");

            texto.text = "";
            letras.Reset();
        }

        void ChecktOverflowing()
        {
            if (texto.isTextOverflowing && fadeMenu.timerOn.Chck)
            {
                letras.SetMultiply(1);

                if (tiempoEntrePaginas > 0)
                {
                    if (entrePaginas.Chck && texto.text != string.Empty)
                    {
                        entrePaginas.Reset();
                        letras.Stop();
                        timerToHide?.Stop();
                    }
                    return;
                }

                var index = texto.text.IndexOfAny(saltos) + 1;

                if (index > 0)
                {
                    texto.text = texto.text.Substring(index);

                    final = final.Substring(final.IndexOfAny(saltos) + 1);
                }
                else if (texto.text.Length > 0)
                {
                    final = final.Replace(texto.text, "");
                    texto.text = "";
                }
            }
        }

        void Write()
        {
            if (texto.text == final && final != "")
            {
                final = "";
            }
            else if (final != "")
            {
                string sum = final[texto.text.Length].ToString();

                if (sum == "<")
                {
                    sum = final.Substring(texto.text.Length, final.IndexOf('>', texto.text.Length) - texto.text.Length + 1);
                }

                texto.text += sum;
                timerToHide?.Reset();
            }

            texto.ForceMeshUpdate();

            ChecktOverflowing();
        }

        public void Init()
        {
            fadeMenu.alphas += FadeMenu_alphas;

            fadeMenu.Init();

            letras = TimersManager.Create(tiempoEntreLetras, Write).SetLoop(true).Stop();

            if (tiempoParaDesaparecer > 0)
                timerToHide = TimersManager.Create(tiempoParaDesaparecer, () =>
                {
                    fadeMenu.end += FadeMenu_end;
                    SetFade(0);
                    letras.Stop();
                });

            entrePaginas = TimersManager.Create(tiempoEntrePaginas,EntrePaginasFixed, EntrePaginas).Stop().SetInitCurrent(0);

            System.Func<Color?, Color> func = (color) =>
            {
                if (color == null)
                    return texto.color;

                texto.color = texto.color.ChangeColorCopy((Color)color);

                return texto.color;
            };

            wow.Init(func);
            cancel.Init(func);
            complete.Init(func);
        }

        private void FadeMenu_end()
        {
            texto.text = "";
            off?.Invoke();
        }
    }
}

