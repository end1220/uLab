
using UnityEngine;
using UnityEngine.UI;
using Lite;


public class StartUp : MonoBehaviour, IListener
{
	string msgText = "";

	void Awake()
	{
		App.Instance.Initialize();

		App.eventManager.RegisterListener(MessageDefine.UPDATE_MESSAGE, this);
		App.eventManager.RegisterListener(MessageDefine.UPDATE_EXTRACT, this);
		App.eventManager.RegisterListener(MessageDefine.UPDATE_DOWNLOAD, this);
		App.eventManager.RegisterListener(MessageDefine.UPDATE_PROGRESS, this);

		//InputField field = null;
		//field.text;
	}

	void Start()
	{
		App.gameManager.CheckResource();
	}

	void OnGUI()
	{
		GUI.Label(new Rect(20, 20, 960, 500), msgText);
	}

	public void OnMessage(Message msg)
	{
		switch (msg.name)
		{
			case MessageDefine.UPDATE_MESSAGE:
				msgText = msg.body.ToString();
				break;
			case MessageDefine.UPDATE_EXTRACT:
				msgText = msg.body.ToString();
				break;
			case MessageDefine.UPDATE_DOWNLOAD:
				msgText = msg.body.ToString();
				break;
			case MessageDefine.UPDATE_PROGRESS:
				msgText = msg.body.ToString();
				break;
		}
	}

	void OnDestroy()
	{
		App.eventManager.UnregisterListener(MessageDefine.UPDATE_MESSAGE, this);
	}

}
