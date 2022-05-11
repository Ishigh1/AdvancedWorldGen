using System;
using AdvancedWorldGen.CustomSized;

namespace AdvancedWorldGen.UI.InputUI.Number;

public class ConfigNumberTextBox<T> : NumberTextBox<T> where T : IConvertible, IComparable
{
	public Params Params;

	public ConfigNumberTextBox(Params @params, string name, T min, T max, string? localizationPath = null) : base(name, min, max)
	{
		Params = @params;
		LocalizationPath = localizationPath;

		CreateUIElement();
	}

	public override T Value
	{
		get => (T)Params.Data[Name];
		set => Params.Data[Name] = value;
	}
}