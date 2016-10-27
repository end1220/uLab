

using Locke;
using Locke.ui;
using UnityEngine;
using UnityEngine.UI;

public class ShopWindow : IWindow
{
	private static string[] buttonNames = new string[5] { "bag", "role", "shop", "tip", "dialog" };

	protected override void OnInit()
	{
		for (int i = 0; i < 5; i++)
		{
			Button button = this.FindWidget<Button>("buttons/button" + (i + 1));
			button.transform.GetComponentInChildren<Text>().text = buttonNames[i];
			UIEventListener.Get(button.gameObject).onClick = OnButtonClick;
		}

		Button back = FindWidget<Button>("buttons/back");
		back.onClick.AddListener(OnClick);
	}

	protected override void OnExit(IContext context)
	{
	}

	protected override void OnPause(IContext context)
	{
	}

	protected override void OnResume(IContext context)
	{
	}

	private void OnButtonClick(GameObject go)
	{
		int index = int.Parse(go.name.Substring(go.name.Length - 1));

		if (index == 1)
		{
			App.uiManager.OpenWindow(GameUI.bag);
		}
		else if (index == 2)
		{
			App.uiManager.OpenWindow(GameUI.role);
		}
		else if (index == 3)
		{
			App.uiManager.OpenWindow(GameUI.shop);
		}
		else if (index == 4)
		{
			App.uiManager.OpenWindow(GameUI.tip);
		}
		else if (index == 5)
		{
			App.uiManager.OpenWindow(GameUI.dialog);
		}
	}

	private void OnClick()
	{
		App.uiManager.CloseWindow(this);
	}

}
