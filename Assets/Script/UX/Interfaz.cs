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

        public void Objective(string str)
        {
            objectiveText.ClearMsg();
            objectiveText.WowEffect();
            objectiveText.ShowMsg("Objetivos:\n" + str);
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
    public class TextCompleto : Init
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

                timerIn = TimersManager.Create(originalColor, grad.Evaluate(0), timeIn, Color.Lerp, (color)=> actionToApplyGrad(color));

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

        static char[] saltos = new char[] { '\n', '.' , '!', '?'};

        public event System.Action on;

        public event System.Action off;

        public string name => texto.name;

        public float velocityMultiply => letras.Multiply;

        public string text => texto.text;

        private void FadeMenu_alphas(float obj)
        {
            texto.color = texto.color.ChangeAlphaCopy(obj);
        }

        public void AcelerateMsg(float velocity)
        {
            letras.SetMultiply(velocity);

            if(!entrePaginas.Chck)
                entrePaginas.SetInitCurrent(0);
            
            if (final==string.Empty)
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

            if(lastIndex>=0)
                final = final.Replace(texto.text.Substring(0,lastIndex+1), "").Trim();
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

                if (tiempoEntrePaginas>0)
                {
                    if(entrePaginas.Chck && texto.text != string.Empty)
                    {
                        entrePaginas.Reset();
                        letras.Stop();
                        timerToHide?.Stop();
                    }
                    return;
                }

                var index = texto.text.IndexOfAny(saltos) + 1;

                if(index>0)
                {
                    texto.text = texto.text.Substring(index);

                    final = final.Substring(final.IndexOfAny(saltos) + 1);
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
                timerToHide?.Reset();
            }

            ChecktOverflowing();
        }

        public void Init()
        {
            fadeMenu.alphas += FadeMenu_alphas;

            fadeMenu.Init();

            letras = TimersManager.Create(tiempoEntreLetras, Write).SetLoop(true).Stop();

            if(tiempoParaDesaparecer>0)
                timerToHide = TimersManager.Create(tiempoParaDesaparecer, () => 
                {
                    fadeMenu.end += FadeMenu_end;
                    SetFade(0);
                    letras.Stop();
                });

            entrePaginas = TimersManager.Create(tiempoEntrePaginas, EntrePaginasFixed, EntrePaginas).Stop().SetInitCurrent(0);

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