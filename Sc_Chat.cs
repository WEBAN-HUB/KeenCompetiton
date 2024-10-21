using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Sc_Chat : MonoBehaviour
{
    public static Sc_Chat Instance;

    public InputField chatInputField;
    public RectTransform chatRectTransform;
    public Text chatText;
    public ScrollRect chatScrollRect;

    public Text userText;
    public RectTransform userRectTransform;
    public ScrollRect userScrollRect;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    /// <summary>
    /// 채팅의 내용을 업데이트한다.
    /// </summary> 
    /// <param name="data> 입력받은 string 데이터</param>
    public void ShowMessage(string data)
    {
        chatText.text += chatText.text == "" ? data : "\n" + data;
        Fit(chatText.GetComponent<RectTransform>());
        Fit(chatRectTransform);
        Invoke("ScrollDelay",0.03f);
    }
    /// <summary>
    /// 유저리스트의 내용을 업데이트한다.
    /// </summary> 
    /// <param name="data> 입력받은 User List의 string 데이터</param>
    public void ShowUserName(string data)
    {
        userText.text = data;
        Fit(userText.GetComponent<RectTransform>());
        Fit(userRectTransform);
        Invoke("UserScrollDelay", 0.03f);
    }

    public void ClearMessageAndUser()
    {
        chatText.text = "";
        userText.text = "";
    }

    /// <summary>
    /// 채팅내용에 맞는 Rect 크기를 재정의하기 위한 함수
    /// </summary> 
    void Fit(RectTransform rect)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }
    /// <summary>
    /// 채팅 스크롤을 가장 최신 것으로 보여주기 위한 함수
    /// </summary> 
    void ScrollDelay()
    {
        chatScrollRect.verticalScrollbar.value = 0;
    }
    /// <summary>
    /// 유저 리스트를 가장 최신 것부터 보여주기 위한 함수
    /// </summary> 
    void UserScrollDelay()
    {
        userScrollRect.verticalScrollbar.value = 0;
    }

}
