using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{

    [Serializable]
    public class Talk
    {
        public Sprite talkerSprite;
        public string talkerName;
        public string text;

        public Talk(Sprite talkerSprite, string talkerName, string text)
        {
            this.talkerSprite = talkerSprite;
            this.talkerName = talkerName;
            this.text = text;
        }
    }

    //[SerializeField] private Talk[] talks;
    [SerializeField] private TMP_Text talkerTmp;
    [SerializeField] private Image talkerImage;
    [SerializeField] private TMP_Text textTmp;
    [SerializeField] private Button buttonNext;
    [SerializeField] private TMP_Text buttonNextTmp;

    /*
    private void Start()
    {
        Talk[] talks = new[]
        {
            new Talk(Resources.Load<Sprite>("Sprites/cha"), "Talker", "texto")
        };
        Setup(talks);
    }
    */

    private Talk[] _talks;
    private int _step = 0;
    public void Setup(Talk[] talks)
    {
        buttonNext.onClick.RemoveAllListeners();
        buttonNext.onClick.AddListener(() => ClickedNextText());
        //if (talkerSprite != null) talkerImage.sprite = talkerSprite;
        _talks = talks;
        _step = -1;
        ClickedNextText();
    }

    private void ClickedNextText()
    {
        if (_coro != null) StopCoroutine(_coro);
        _step++;
        _coro = StartCoroutine(ShowText(_talks[_step]));
        if (_step+1 >= _talks.Length) buttonNext.gameObject.SetActive(false);
    }

    private Coroutine _coro;

    private IEnumerator ShowText(Talk talk)
    {
        //talkerImage.sprite = talk.talkerSprite;
        talkerTmp.text = talk.talkerName;
        if (talk.talkerSprite != null) talkerImage.sprite = talk.talkerSprite;
        for (int i = 0; i < talk.text.Length; i++)
        {
            string beforeText = talk.text.Substring(0, i);
            string newText = beforeText + "<color=#00000000>" + talk.text.Substring(i);
            textTmp.text = newText;
            yield return new WaitForSeconds(0.03f);
        }
        yield return null;
        textTmp.text = talk.text;
    }

}
