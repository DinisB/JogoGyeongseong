using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;

public class MessageScript : MonoBehaviour
{
    private JArray messages;
    [SerializeField] private TMP_Text uiText;
    [SerializeField] private float timer;
    [SerializeField] private int start;
    [SerializeField] private int end;
    [SerializeField] private Image imageBox;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TextAsset textFile = Resources.Load<TextAsset>("messageText"); // vai buscar json das mensagens
        string jsonString = textFile.text;
        messages = JArray.Parse(jsonString); // dá parse
        ShowText(start, timer);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        { //vai para proxima mensgaem
            if (start < end)
            {
                start++;
                ShowText(start, timer);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void StartMessageText(int start_t, int end_t)
    {
        gameObject.SetActive(true);
        start = start_t;
        end = end_t;
        ShowText(start, timer);
    }

    public void ShowText(int num, float timer)
    {
        StopAllCoroutines();
        StartCoroutine(TypeText(num, timer));
    }

    private System.Collections.IEnumerator TypeText(int num, float timer)
    {
        string name = messages[num]["name"]?.ToString(); // mete o name como nome
        string message = messages[num]["text"]?.ToString(); // guarda a message
        string icon = messages[num]["icon"]?.ToString();
        imageBox.sprite = Resources.Load<Sprite>("Icons/" + icon); // dá load dos icones para a ui
        StringBuilder textBuilder = new StringBuilder();
        textBuilder.Append(name);
        foreach (char c in message) // dá load de cada message e mostra os chars no texto
        {
            textBuilder.Append(c);
            uiText.text = textBuilder.ToString();
            yield return new WaitForSeconds(timer);
        }
    }
}
