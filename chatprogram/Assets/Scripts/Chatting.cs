using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Text;
using System;

public class Chatting : MonoBehaviour
{
	InputField inputNick;
	InputField inputMessage;

	[SerializeField]
	private Image[] imoticon;

	private TransportTCP transport;

	private string hostAddress = "";
	private const int myPort = 50765;

	private string sendComment = "";
	private string prevComment = "";

	private bool isServer;

	private string myId = ""; //자신 아이디
	private int curIdNum = 0; // current Number
	private int myIdNum; // my Number

	private List<string>[] messages; // Each player Chatting
	private List<Image>[] imoticons;
	private int[] chat; // 0 : message 1: imooticon


	public Image MainTitleImg;
	public Image BGImg;

	private static int maxChatMember = 4;
	private static int message_line = 18;

	private ChatState myState = ChatState.HOST_TYPE_SELECT;

	enum ChatState
	{
		HOST_TYPE_SELECT = 0,   // 방 선택.
		CHATTING,               // 채팅 중.
		LEAVE,                  // 나가기.
		ERROR,                  // 오류.
	}

	[SerializeField]
	private GameObject ErrorPanel;



	void Start()
	{
		IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
		IPAddress serverAddress = hostEntry.AddressList[0];
		Debug.Log(hostEntry.HostName);
		hostAddress = serverAddress.ToString();

		GameObject go = new GameObject("Network");
		transport = go.AddComponent<TransportTCP>();

		transport.RegisterEventHandler(OnEventHandling);

		messages = new List<string>[maxChatMember];
		for (int i = 0; i < maxChatMember; ++i)
		{
			messages[i] = new List<string>();
		}
	}

	private void Update()
	{
		switch (myState)
		{
			case ChatState.HOST_TYPE_SELECT:
				for (int i = 0; i < maxChatMember; ++i)
				{
					messages[i].Clear();
				}
				break;

			case ChatState.CHATTING:
				UpdateChatting();
				break;

			case ChatState.ERROR:
				UpdateError();
				break;
		}
	}

	private void GetIdBtn()
    {
		myId = inputNick.text;
		
		myIdNum = curIdNum;
		curIdNum++;

		Debug.Log(myIdNum);
	}

	/// <summary>
    /// make Chatting Room
    /// </summary>
	public void MakeChatServer()
	{
		transport.StartServer(myPort, 1);
		myState = ChatState.CHATTING;
		isServer = true;
	}

	/// <summary>
    /// Connect Chatting Room
    /// </summary>
	public void ConnectChatServer()
	{
		bool ret = transport.Connect(hostAddress, myPort);

		if (ret)
		{
			myState = ChatState.CHATTING;
		}
		else
        {
			myState = ChatState.ERROR;
        }

	}

	private void AddImoticon(ref List<string> messages, Image image)
	{
		while (messages.Count >= message_line)
		{

		}
	}


	private void AddMessage(ref List<string> messages, string str)
	{
		while (messages.Count + imoticons.Length >= message_line)
		{
			messages.RemoveAt(0);
		}

		messages.Add(str);
	}


	
	/// <summary>
    /// Get Id from InputField
    /// </summary>
	public void GetId()
    {
		myId = inputNick.text;
    }


	private void UpdateChatting()
    {
		byte[] buffer = new byte[1000];

		int recvSize = transport.Receive(ref buffer, buffer.Length);

		if(recvSize > 0)
        {
			string message = Encoding.UTF8.GetString(buffer);

			int id = (isServer) ? 1 : 0;
			AddMessage(ref messages[id], message);
			
        }
    }

	/// <summary>
    /// Send Message Button
    /// </summary>
	public bool IsSentBtn()
    {
		return true;
    }

	public void OutRoomBtn()
    {
		if(isServer)
        {
			transport.StopServer();
        }
		else
        {

        }
	}

	private void ChattingGUI()
	{

		sendComment = inputMessage.text;

		bool isSent = IsSentBtn();
		if (Event.current.isKey &&
			Event.current.keyCode == KeyCode.Return)
		{
			if (sendComment == prevComment)
			{
				isSent = true;
				prevComment = "";
			}
			else
			{
				prevComment = sendComment;
			}
		}

		if (isSent == true)
		{
			string message = myId + " : " + sendComment + "[" + DateTime.Now.ToString("HH:mm") + "] ";
			byte[] buffer = Encoding.UTF8.GetBytes(message);
			transport.Send(buffer, buffer.Length);
			AddMessage(ref messages[myIdNum], message);
			sendComment = "";
		}

		
	}

	/// <summary>
    /// Player Leave Chat room
    /// </summary>
	private void UpdateLeave()
    {

    }

	/// <summary>
    /// Connect Error
    /// </summary>
	private void UpdateError()
    {
		ErrorPanel.SetActive(true);
	}

	/// <summary>
    /// Close Error Panel
    /// </summary>
	public void ErrorBackBtn()
    {
		ErrorPanel.SetActive(false);
		myState = ChatState.HOST_TYPE_SELECT;
    }


	public void OnEventHandling(NetEventState state)
	{
		switch (state.type)
		{
			case NetEventType.Connect:
				if (transport.IsServer())
				{
					AddMessage(ref messages[1], "콩장수가 입장했습니다.");
				}
				else
				{
					AddMessage(ref messages[0], "두부장수와 이야기할 수 있습니다.");
				}
				break;

			case NetEventType.Disconnect:
				if (transport.IsServer())
				{
					AddMessage(ref messages[0], "콩장수가 나갔습니다.");
				}
				else
				{
					AddMessage(ref messages[1], "콩장수가 나갔습니다.");
				}
				break;
		}
	}
}

