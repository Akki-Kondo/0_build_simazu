using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;

public class MessageWindow : MonoBehaviour
{
    public string Message = "";
    public float TextSpeedPerChar = 1000 / 10f;�@//��������̂P����������̑���
    [Min(1)] public float SpeedUpRate = 4f;�@//�{�^�����������ɕ�������̃X�s�[�h�A�b�v�䗦
    [Min(1)] public int MaxLineCount = 3;�@//�\���ł���ő�s��

    public bool IsEndMessage { get; private set; } = true;�@//���b�Z�[�W���I���������ǂ����̃t���O
    Transform TextRoot;
    Text TextTemplate;

    public InputActionAsset playerControls;
    private List<InputAction> anyKeyActions;    //InputSystem�p�ɐV�����A�N�V������ǉ�

    private void Awake()
    {
        TextRoot = transform.Find("Panel");
        TextTemplate = TextRoot.Find("TextTemplate").GetComponent<Text>();

        TextTemplate.gameObject.SetActive(false);
        gameObject.SetActive(false);

        var gameplayActionMap = playerControls.FindActionMap("PlayerControls");
        anyKeyActions = new List<InputAction> { gameplayActionMap.FindAction("Any") }; // �擾

    }

    private void OnEnable()
    {
        foreach (var action in anyKeyActions)
        {
            action.Enable();
        }
    }

    private void OnDisable()
    {
        if (anyKeyActions != null)
        {
            foreach (var action in anyKeyActions)
            {
                action?.Disable();
            }
        }
    }


    public void StartMessage(string message)�@//���b�Z�[�W�A�j���[�V�����̊J�n
    {
        Message = message;
        StopAllCoroutines();
        gameObject.SetActive(true);
        StartCoroutine(MessageAnimation());
    }

    IEnumerator MessageAnimation()�@//���b�Z�[�W�A�j���[�V�����̕\���Ƒ��x
    {
        IsEndMessage = false;
        DestroyLineText();

        var lines = Message.Split('\n');
        var lineCount = 0;
        var textObjs = new List<Text>();

        foreach (var line in lines)
        {
            lineCount++;
            if (lineCount >= MaxLineCount)
            {
                Object.Destroy(textObjs[0].gameObject);
                textObjs.RemoveAt(0);
            }
            var lineText = Object.Instantiate(TextTemplate, TextRoot);
            lineText.gameObject.SetActive(true);
            lineText.text = "";
            textObjs.Add(lineText);

            for (var i = 0; i < line.Length; ++i)
            {
                lineText.text += line[i];
                var speed = TextSpeedPerChar / (anyKeyActions.Any(action => action.triggered) ? SpeedUpRate : 2);
                yield return new WaitForSeconds(speed);
            }
        }

        yield return new WaitUntil(() => anyKeyActions.Any(action => action.triggered));
        IsEndMessage = true;
        gameObject.SetActive(false);
    }

    void DestroyLineText()
    {
        foreach (var text in TextRoot.GetComponentsInChildren<Text>().Where(_t => _t != TextTemplate))
        {
            Object.Destroy(text.gameObject);
        }
    }
}