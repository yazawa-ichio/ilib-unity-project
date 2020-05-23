using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ILib.ScWidgets;

public class uGUIBuilderTest : MonoBehaviour
{
	uGUIBuilder m_Builder;

	private void Start()
	{
		var m_Canvas = new ScWindow()
		{
			Layout = new Layout(Vector2.zero, new Vector2(1280, 720)),
			Children = new IScWidget[]
			{
				new ScTexture()
				{
					Layout = new HorizontalStretchLayout()
					{
						Height = 50,
						Margin = new Vector2(5,5),
						PosY = 5,
						Anchor = LayoutAnchor.Top,
					},
					Texture = Texture2D.whiteTexture,
					Child = new ScWidget()
					{
						Layout = new Layout(Vector2.zero, new Vector2(200,30),LayoutAnchor.RightTop),
						Child = new ScText()
						{
							Text = "Test Text.",
							TextAnchor = TextAnchor.MiddleLeft,
						}
					}
				},
				new ScTexture()
				{
					Layout = new Layout(Vector2.zero, new Vector2(200,50),LayoutAnchor.RightBottom),
					Texture = Texture2D.whiteTexture,
					Child = new ScButton()
					{
						Layout = new StretchLayout()
						{
							Margin = new RectOffset(5,5,5,5)
						},
					}
				},
				new ScToggle()
				{
					Layout = new Layout(Vector2.zero, new Vector2(50,50),LayoutAnchor.LeftBottom),
				},
			}
		};
		m_Builder = new uGUIBuilder();
		m_Builder.Build(m_Canvas);
	}

	private void Update()
	{

	}


}