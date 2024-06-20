using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatisticsModule : MonoBehaviour
{
    [SerializeField]
    Image banner;

    [SerializeField]
    TextMeshProUGUI title;

    [SerializeField]
    TextMeshProUGUI content;

    public StatisticsModule SetText(string _title, string _content)
    {
        title.text = _title;
        content.text = _content;

        return this;
    }

    public StatisticsModule SetBanner(Sprite _sprite)
    {
        banner.sprite = _sprite;

        return this;
    }
}
