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

        [SerializeField] float tiempoParaDesaparecer = 3;

        [SerializeField] float width;

        [SerializeField] float height;

        [SerializeField] FadeOnOff fadeMenu;

        [SerializeField] bool useAutomaticGradients;

        [SerializeField] GradientOverTime wow;

        [SerializeField] GradientOverTime cancel;

        [SerializeField] GradientOverTime complete;

        [Header("Para debug")]
        [SerializeReference] Timer timerToHide;

        [SerializeReference] Timer writeTimer;

        static char[] saltos = new char[] { '\n', '.', '!', '?' };

        public event System.Action on;

        public event System.Action off;

        public string text => texto.text;

        private void FadeMenu_alphas(float obj)
        {
            texto.color = texto.color.ChangeAlphaCopy(obj);
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
            writeTimer.Stop();
            final = string.Empty;
            texto.text = string.Empty;
        }

        public void CompleteMsg()
        {
            if (final.IsEmpty() && text.IsEmpty())
            {
                return;
            }

            if(tiempoParaDesaparecer>0 && !timerToHide.Freeze)
            {
                //adelanto el tiempo en caso que ya este todo escrito
                timerToHide.SetInitCurrent(0);
            }
            else
            {
                //En caso que este escribiendo, lo completo todo
                CompleteText();
            }
        }


        /// <summary>
        /// Muestra un mensaje directamente en el campo, no checkea su largo
        /// </summary>
        /// <param name="msg"></param>
        public void ShowMsg(string msg)
        {
            final = msg;
            texto.text = msg;
            On();
        }

        /// <summary>
        /// Aniade un mensaje que se mostrara de forma ordenada y escrito letra por letra
        /// </summary>
        /// <param name="msg"></param>
        public void AddMsg(string msg)
        {
            if (final.IsEmpty())
            {
                writeTimer.SetMultiply(1);
                msg = texto.text + msg;
            }

            final += msg + "\n";

            On();
        }

        void On()
        {
            fadeMenu.end -= FadeMenu_end;
            SetFade(1);

            if (writeTimer.total > 0)
                writeTimer.Reset();
            else
                CompleteText();

            on?.Invoke();
        }

        void SetFade(float end)
        {
            fadeMenu.SetFade(texto.color.a, end);
        }

        void Write()
        {
            if(!IsTextOverflowing())
            {
                if (texto.text == final && !final.IsEmpty())
                {
                    final = string.Empty;
                    writeTimer.Stop();
                    timerToHide?.Reset();
                }
                else if (!final.IsEmpty())
                {
                    string sum = final[texto.text.Length].ToString();

                    if (sum == "<")
                    {
                        sum = final.Substring(texto.text.Length, final.IndexOf('>', texto.text.Length) - texto.text.Length + 1);
                    }

                    texto.text += sum;
                }
            }
            else
            {
                Overflowing();
            }
        }

        /// <summary>
        /// Escribo todo el texto posible en el cuadro del texto
        /// </summary>
        void CompleteText()
        {
            int lenght = 0;

            texto.text = final;

            while (IsTextOverflowing())
            {
                var index = texto.text.Remove(texto.text.Length - 1).LastIndexOfAny(saltos);

                if (index > 0)
                {
                    texto.text = texto.text.Substring(0, index + 1);

                    if (texto.text.Length == lenght)
                        break;

                    lenght = texto.text.Length;
                }
                else
                {
                    Debug.LogWarning("No se encontro como cortar el texto");
                    break;
                }
            }

            //dado que debo continuar con el ciclo logico, la forma mas sencilla es como si siempre este overrideando el texto
            //para asi pasar pagina o etc
            Overflowing();
        }

        /// <summary>
        /// Que hago en caso de que el texto este overflowing
        /// </summary>
        void Overflowing()
        {
            if (tiempoParaDesaparecer > 0)
            {
                if (!text.IsEmpty())
                {
                    writeTimer.Stop();
                    timerToHide.Reset();
                }
                return;
            }

            NextPage();
        }

        /// <summary>
        /// Que hago cuando paso la pagina
        /// </summary>
        void NextPage()
        {
            var lastIndex = texto.text.LastIndexOfAny(saltos);

            if (lastIndex >= 0)
                final = final.Replace(texto.text.Substring(0, lastIndex + 1), string.Empty).Trim();
            //else
            //  final = final.Replace(texto.text, "");

            texto.text = string.Empty;

            timerToHide?.Stop();

            if (final.IsEmpty())
            {
                Hide();
            }
            else
            {
                if (tiempoEntreLetras <= 0)
                    CompleteText();
                else
                    writeTimer.Reset();
            }    
        }

        public void Init()
        {
            fadeMenu.alphas += FadeMenu_alphas;

            fadeMenu.Init();

            writeTimer = TimersManager.Create(tiempoEntreLetras, Write).SetLoop(true).Stop();

            if (tiempoParaDesaparecer > 0)
                timerToHide = TimersManager.Create(tiempoParaDesaparecer, () =>
                {
                    if(!final.IsEmpty())//dependiendo si tengo algo por escribir o no, realizo acciones distintas
                    {
                        NextPage();
                    }
                    else
                    {
                        Hide();
                    }
                }).Stop();

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

        private void Hide()
        {
            fadeMenu.end += FadeMenu_end;
            SetFade(0);
            writeTimer.Stop();
        }

        private void FadeMenu_end()
        {
            texto.text = string.Empty;
            off?.Invoke();
        }

        private bool IsTextOverflowing()
        {
            float preferedHeight = texto.GetPreferredValues(texto.text.ClearRichText(), width, height).y;
            //Debug.Log($"{preferedHeight} >= {height} : {preferedHeight >= height}" );

            return preferedHeight >= height;
        }

        private void OnValidate()
        {
            if(width==0)
                width = texto?.rectTransform.rect.width ?? 0;
            if(height==0)
                height = texto?.rectTransform.rect.height ?? 0;
        }
    }
}

