namespace AdvancedWorldGen.UI.InputUI;

public class EditableText<T> : FocusElement where T : IConvertible, IComparable
{
	public string? CurrentContent;
	public int FrameNumber;

	public NumberTextBox<T> ParentBox;
	public int Shift;

	public EditableText(NumberTextBox<T> parent)
	{
		ParentBox = parent;
		FrameNumber = 0;
	}

	public override string DisplayText => ParentBox.Value.ToString()!;

	public override void Focus()
	{
		base.Focus();

		CurrentContent = DisplayText;
		Shift = 0;
		Main.clrInput();
	}

	public override void Unfocus()
	{
		base.Unfocus();

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
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		base.DrawSelf(spriteBatch);

		if (IsTheCurrentFocus)
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

				string displayString = CurrentContent[..^Shift] + "|" + CurrentContent[^Shift..];
				DrawText(spriteBatch, displayString);
			}
		}
	}
}