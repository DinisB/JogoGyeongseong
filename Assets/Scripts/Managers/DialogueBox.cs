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
    private Canvas _selfCanvas;

    private void Awake()
    {
        _selfCanvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        _selfCanvas.enabled = false;
        /*
        Talk[] talks = new[]
        {
            new Talk(Resources.Load<Sprite>("Sprites/cha"), "Talker", "texto"),
            new Talk(Resources.Load<Sprite>("Sprites/jang"), "Talker2", "texto2")
        };
        Setup(talks);
        */
    }

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
        //if (talkerSprite != null) talkerImage.sprite = talkerSprite;
        _talks = talks;
        _step = -1;
        ClickedNextText();
        _selfCanvas.enabled = true;
    }

    public void ClickedNextText()
    {
        if (_coro != null) StopCoroutine(_coro);
        _step++;
        if (_step + 1 > _talks.Length)
        {
            _selfCanvas.enabled = false;
            return;
        }
        _coro = StartCoroutine(ShowText(_talks[_step]));
        if (_step + 1 >= _talks.Length)
        {
            buttonNextTmp.text = "Close";
            // buttonNext.gameObject.SetActive(false);
        }
        else
        {
            buttonNextTmp.text = "Next";
        }
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
