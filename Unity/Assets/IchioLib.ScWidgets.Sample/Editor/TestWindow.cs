using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ILib.ScWidgets;
using System.Linq;

public class TestWindow : EditorWindow
{
	[MenuItem("Tools/ScWidgetTest")]
	static void Open()
	{
		GetWindow<TestWindow>();
	}

	ScWindow m_Canvas;
	SkinIMGUIDrawer m_Drawer;

	void OnEnable()
	{
		m_Canvas = new ScWindow()
		{
			Layout = new StretchLayout(),
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
					Child = new ScText()
					{
						Text = "Test Text.",
						TextAnchor = TextAnchor.MiddleLeft,
					}
				},
				new ScTexture()
				{
					Layout = new Layout(Vector2.zero, new Vector2(200,50),LayoutAnchor.RightBottom),
					Texture = Texture2D.blackTexture,
					Children = new IScWidget[]
					{
						new ScButton()
						{
							Layout = new StretchLayout()
							{
								Margin = new RectOffset(5,5,5,5)
							},
							OnClick = () => { Debug.Log("OnClick1"); },
							Child = new ScText()
							{
								Text = "ボタン1",
								FontSize = 14 ,
								TextAnchor = TextAnchor.MiddleCenter,
							}
						},
						new ScButton()
						{
							Layout = new StretchLayout()
							{
								Margin = new RectOffset(5,5,5,5)
							},
							OnClick = () => { Debug.Log("OnClick2"); },
							Child = new ScText()
							{
								Text = "ボタン2",
								FontSize = 14 ,
								TextAnchor = TextAnchor.MiddleCenter,
							}
						},
						new ScButton()
						{
							Layout = new StretchLayout()
							{
								Margin = new RectOffset(5,5,5,5)
							},
							OnClick = () => { Debug.Log("OnClick3"); },
							Child = new ScText()
							{
								Text = "ボタン3",
								FontSize = 14 ,
								TextAnchor = TextAnchor.MiddleCenter,
							}
						}
					}
				},
				new ScToggle()
				{
					Layout = new Layout(Vector2.zero, new Vector2(64,32),LayoutAnchor.LeftBottom),
				},
				new ScSlider()
				{
					Layout = new Layout(Vector2.zero, new Vector2(250,150),LayoutAnchor.Right),
				},
				new ScInputField()
				{
					Input = "test",
					FontSize = 18,
					Layout = new Layout(new Vector2(0,50), new Vector2(250, 30),LayoutAnchor.RightTop),
				},
				new ScCheckBox()
				{
					Layout = new Layout(new Vector2(0,-50), new Vector2(100, 100),LayoutAnchor.RightBottom),
				},
			}
		};

		var grid = new ScGrid()
		{
			HorizontalCount = 3,
			VerticalCount = 3,
			Padding = new Vector2(5, 5),
			Children = new[]{
				new ScTexture()
				{
					Texture = Texture2D.whiteTexture,
					Child = new ScButton()
					{
						Layout = new StretchLayout() { Margin = new RectOffset(5,5,5,5)},
					}
				},
				new ScTexture()
				{
					Texture = Texture2D.whiteTexture,
					Child = new ScButton()
					{
						Layout = new StretchLayout() { Margin = new RectOffset(5,5,5,5)},
					}
				},
				new ScTexture()
				{
					Texture = Texture2D.whiteTexture,
					Child = new ScButton()
					{
						Layout = new StretchLayout() { Margin = new RectOffset(5,5,5,5)},
					}
				},
				new ScTexture()
				{
					Texture = Texture2D.whiteTexture,
					Child = new ScButton()
					{
						Layout = new StretchLayout() { Margin = new RectOffset(5,5,5,5)},
					}
				},
				new ScTexture()
				{
					Texture = Texture2D.blackTexture,
					Child = new ScButton()
					{
						Layout = new StretchLayout() { Margin = new RectOffset(5,5,5,5)},
					}
				},
			},
		};

		List<ScWidget> list = new List<ScWidget>(1000);
		for (int i = 0; i < 100; i++)
		{
			list.Add(new ScRadioButton()
			{
				Layout = new StretchLayout()
				{
					Margin = new RectOffset(0, 0, 2, 2),
				},
				Child = new ScText()
				{
					Layout = new StretchLayout()
					{
						Margin = new RectOffset(30, 0, 0, 0),
					},
					Text = "Test Index : " + i,
					TextAnchor = TextAnchor.MiddleLeft,
					FontSize = 16,
				},
				Extend = ExtendWidth.Get(210)
			});
		}

		//		m_Canvas.Add(grid);
		m_Canvas.Add(new ScScrollView()
		{
			Layout = new Layout(new Vector2(50, 100), new Vector2(200, 200), LayoutAnchor.LeftTop),
			Height = 34,
			Children = list.ToArray(),
		});

		m_Drawer = new SkinIMGUIDrawer(m_Canvas, AssetDatabase.LoadAssetAtPath<IMGUIWidgetSkin>("Assets/IchioLib.ScWidgets/Runtime/SkinIMGUI/IMGUIWidgetSkin.asset"));
		m_Drawer.IsEditorGUI = true;
	}

	void OnGUI()
	{
		m_Drawer.Draw(new Rect(0, 0, position.width, position.height));
	}
}