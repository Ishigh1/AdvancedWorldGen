using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;

namespace AdvancedWorldGen.UI.InputUI;

public class EditableText<T> : UIElement, IEditableText where T : IConvertible, IComparable
{
	public static IEditableText? CurrentFocus;
	public string? CurrentContent;
	public int FrameNumber;

	public NumberTextBox<T> ParentBox;
	public int Shift;

	public EditableText(NumberTextBox<T> parent)
	{
		ParentBox = parent;
		FrameNumber = 0;
	}

	public void Focus()
	{
		CurrentFocus?.Unfocus();
		CurrentContent = ParentBox.Value.ToString();
		CurrentFocus = this;
		Shift = 0;
		Main.clrInput();
	}

	public void Unfocus()
	{
		if (CurrentContent != null)
		{
			Type type = typeof(T);
			Type[] types = { typeof(string), typeof(T).MakeByRefType() };
			MethodInfo methodInfo = type.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, types)!;
			object[] parameters = { CurrentContent, ParentBox.Value };
			bool isValid = (bool)methodInfo.Invoke(null, parameters)!;
			if (isValid)
			{
				ParentBox.Value = (T)parameters[1];
			}
			else
			{
				parameters[0] = "0";
				methodInfo.Invoke(null, parameters);
				ParentBox.Value = (T)parameters[1];
			}

			if (ParentBox.Value.CompareTo(ParentBox.Min) < 0)
				ParentBox.Value = ParentBox.Min;

			else if (ParentBox.Value.CompareTo(ParentBox.Max) > 0)
				ParentBox.Value = ParentBox.Max;

			CurrentContent = null;
			CurrentFocus = null;
		}
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		if (Main.mouseLeft && Main.mouseLeftRelease)
			if (Parent.IsMouseHovering)
				Focus();
			else
				Unfocus();

		if (CurrentContent != null)
		{
			PlayerInput.WritingText = true;
			Main.instance.HandleIME();

			string oldString = CurrentContent[..^Shift];
			string newString = Main.GetInputText(oldString);
			if (newString != oldString)
			{
				newString += CurrentContent[^Shift..];
				object[] parameters = { newString, ParentBox.Value };
				Type type = typeof(T);
				Type[] types = { typeof(string), typeof(T).MakeByRefType() };
				MethodInfo methodInfo = type.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, types)!;
				bool isValid = (bool)methodInfo.Invoke(null, parameters)!;
				if (isValid || newString == "")
					CurrentContent = newString;
			}

			if (Main.inputTextEnter)
			{
				Unfocus();
			}
			else //Move cursor
			{
				if (Main.inputText.IsKeyDown(Keys.Left))
				{
					if (FrameNumber++ % 10 == 0)
						Shift = Math.Min(CurrentContent.Length, Shift + 1);
				}
				else if (Main.inputText.IsKeyDown(Keys.Right))
				{
					if (FrameNumber++ % 10 == 0)
						Shift = Math.Max(0, Shift - 1);
				}
				else
				{
					FrameNumber = 0;
				}
			}
		}

		string displayString;
		if (CurrentContent == null)
			displayString = ParentBox.Value.ToString()!;
		else
			displayString = CurrentContent[..^Shift] + "|" + CurrentContent[^Shift..];

		CalculatedStyle space = GetDimensions();
		Vector2 size = Utils.DrawBorderString(spriteBatch, displayString, new Vector2(space.X, space.Y), Color.White);
		Width.Pixels = size.X;
		Height.Pixels = size.Y;
	}
}