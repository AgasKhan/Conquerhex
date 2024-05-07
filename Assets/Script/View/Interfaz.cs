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

        static public Interfaz instance;

        public List<TextCompleto> textC = new List<TextCompleto>();

        [Header("Dialogo")]

        [SerializeField] Image dialogoImage;
        TextCompleto dialogoText;

        float   widthDiag;
        float   heightDiag;

        [Header("Vida")]
        public ImageWidth vida;
        public TextMeshProUGUI textVida;
        public ImageWidth regen;
        public ImageWidth regenMax;

        public TextMeshProUGUI textRegen;
        public ImageWidth regenTimeMax;
        public ImageWidth regenTime;

        [Header("Energia")]
        public Slider energy;
        public Image requirementLeft;
        public Image requirementRight;
        public Transform textPopEnergy;
        public TextMeshProUGUI actualEnergy;

        [Header("Cooldowns")]
        public Image[] basicsCooldowns;
        public Image[] katasCooldowns;
        public Image[] abilitiesCooldowns;

        [Header("Energy")]
        Timer leftEnergy;
        Timer rightEnergy;

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

        public void PopTextDamage(Entity entity, string text)
        {
            //instance["Danio"].AddMsg($"{text} ► {entity.name.Replace("(Clone)", "")}");

            PoolManager.SpawnPoolObject(Vector2Int.up * 2, out TextPop textDamage);

            textDamage.SetText(entity.transform, text);
        }

        private void healthBarUpdate(Health health)
        {
            float nextRegenLifePercentage = health.nextRegenLife / health.maxLife;

            float healthPercentage = health.actualLife / health.maxLife;

            float cooldownRegen = (1 - health.actualCoolDownRegen / health.MaxCoolDownRegen);

            textVida.text = ((int)health.actualLife).ToString();

            regen.FillAmount = health.actualRegen / 100f;

            regenMax.FillAmount = health.maxRegen / 100f;

            textRegen.text = ((int)(health.actualRegen / 100 * health.maxLife)).ToString();

            regenTime.FillAmount = cooldownRegen * nextRegenLifePercentage;

            if (cooldownRegen == 0 || cooldownRegen > 0.05f)
            {
                var aux = (Mathf.Clamp((cooldownRegen - (0.05f)) * 10, 0, 1));

                regenTimeMax.FillAmount = Mathf.Clamp((health.nextRegenLife * aux) / health.maxLife, healthPercentage, 1);
            }

            if(vida.FillAmount > healthPercentage)
            {
                if (cooldownRegen < 0.05f)
                {
                    vida.FillAmount = Mathf.Lerp(vida.FillAmount, healthPercentage, Time.deltaTime);
                }
                else
                {
                    vida.FillAmount = Mathf.Lerp(vida.FillAmount, healthPercentage, Time.deltaTime*10);
                }
            }
            else
            {
                vida.FillAmount = healthPercentage;
            }
        }

        private void EnergyBarUpdate((float energyValue, float diference, float energyActual) str)
        {
            if (Mathf.Abs(str.diference) > 0.5f)
            {
                PoolManager.SpawnPoolObject(Vector2Int.up * 2, out TextPop textEnergy);

                str.diference *= -1;

                string diference = str.diference.ToStringFixed();

                if (str.diference < 0)
                {
                    diference = diference.RichTextColor(Color.red);
                }
                else
                {
                    diference = ("+" + diference).RichTextColor(Color.blue);
                }

                diference=diference.RichText("size", actualEnergy.fontSize.ToStringFixedComma(1));

                textEnergy.SetText(textPopEnergy, diference, Vector2.up * 0.5f * Mathf.Sign(-str.diference), false);
            }

            energy.value = str.energyValue;

            actualEnergy.text = str.energyActual.ToStringFixedComma(1);
        }

        private void EnergyLeft(float energyValue)
        {
            requirementLeft.fillAmount = energyValue;
            leftEnergy.Reset();
        }

        private void EnergyRight(float energyValue)
        {
            requirementRight.fillAmount= energyValue;
            rightEnergy.Reset();
        }

        IEnumerator MyCoroutine(System.Action<bool> end, System.Action<string> msg)
        {
            msg("Interfaz");
            end(true);
            yield return null;
            eventsManager.events.SearchOrCreate<SingleEvent<Health>>(LifeType.all).delegato += healthBarUpdate;

            var aux = eventsManager.events.SearchOrCreate<TripleEvent<(float, float, float), float, float>>("EnergyUpdate");

            aux.delegato += EnergyBarUpdate;
            aux.secondDelegato += EnergyLeft;
            aux.thirdDelegato += EnergyRight;

            leftEnergy =    TimersManager.Create(0.3f, () => requirementLeft.enabled = leftEnergy.current< leftEnergy.total/3 || leftEnergy.current > 2* (leftEnergy.total / 3), ()=> requirementLeft.enabled = false).Stop();
            rightEnergy =   TimersManager.Create(0.3f, () => requirementRight.enabled = rightEnergy.current < rightEnergy.total / 3 || rightEnergy.current > 2 * (rightEnergy.total / 3), () => requirementRight.enabled = false).Stop();
        }

        void Update()
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
       
            LoadSystem.AddPostLoadCorutine(MyCoroutine);
        }

        private void Awake()
        {
            instance = this;

            foreach (var item in textC)
            {
                item.Init();
            }

            dialogoText = SearchTitle("Subtitulo");
        }
    }

    [System.Serializable]
    public class TextCompleto : Init
    {
        [Header("Seteo")]

        [SerializeField] TextMeshProUGUI texto;    
        [field: SerializeField] [field: TextArea(3, 6)] public string final { get; private set; }

        [SerializeField] float tiempoEntreLetras;

        [SerializeField] float tiempoParaDesaparecer = 3;

        [SerializeField]
        FadeOnOff fadeMenu;

        [Header("Para debug")]
        [SerializeReference]
        Timer timer;
        [SerializeReference]
        Timer letras;

        public event System.Action on;

        public event System.Action off;

        public string name => texto.name;

        public string text => texto.text;

        private void FadeMenu_alphas(float obj)
        {
            texto.color = texto.color.ChangeAlphaCopy(obj);
        }

        /// <summary>
        /// Borra el texto del cuadro del mensaje
        /// </summary>
        /// <param name="msg"></param>
        public void ClearMsg()
        {
            final = "";
            texto.text = "";
        }

        /// <summary>
        /// Muestra un mensaje directamente en el campo
        /// </summary>
        /// <param name="msg"></param>
        public void ShowMsg(string msg)
        {
            final = msg;
            texto.text = msg;

            if (timer.Chck)
                On();
            else
                timer.Reset();
        }

        /// <summary>
        /// Aniade un mensaje que se mostrara de forma ordenada y escrito letra por letra
        /// </summary>
        /// <param name="msg"></param>
        public void AddMsg(string msg)
        {
            if (final == "")
                msg = texto.text + msg;

            final += msg + "\n";

            On();
        }

        void On()
        {
            fadeMenu.end -= FadeMenu_end;
            SetFade(1);
            timer.Reset();
            letras.Start();
            on?.Invoke();
        }

        void SetFade(float end)
        {
            fadeMenu.SetFade(texto.color.a, end);
        }

        void ChecktOverflowing()
        {
            if (texto.isTextOverflowing && fadeMenu.timerOn.Chck)
            {

                var aux = new char[] { '\n' , '.'};

                var index = texto.text.IndexOfAny(aux) + 1;

                if(index>0)
                {
                    texto.text = texto.text.Substring(index);

                    final = final.Substring(final.IndexOfAny(aux) + 1);
                }
                else if(texto.text.Length > 0)
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
                    sum =  final.Substring(texto.text.Length, final.IndexOf('>', texto.text.Length) - texto.text.Length + 1);
                }

                texto.text += sum;            
                timer.Reset();
            }
        }

        public void Init()
        {
            fadeMenu.alphas += FadeMenu_alphas;

            fadeMenu.Init();

            letras = TimersManager.Create(tiempoEntreLetras, ChecktOverflowing, Write).SetLoop(true).Stop();

            timer = TimersManager.Create(tiempoParaDesaparecer, () => {

                fadeMenu.end += FadeMenu_end;
                SetFade(0);
                letras.Stop();

                });
        }

        private void FadeMenu_end()
        {
            texto.text = "";
            off?.Invoke();
        }
    }

}