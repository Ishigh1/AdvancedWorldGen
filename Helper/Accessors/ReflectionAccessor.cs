namespace AdvancedWorldGen.Helper.Accessors;

public abstract class ReflectionAccessor<T>
{
	public object? Data;

	public T Value
	{
		get => Get();
		set => Set(value);
	}

	protected abstract void Set(T value);
	protected abstract T Get();

	public static implicit operator T(ReflectionAccessor<T> accessor)
	{
		return accessor.Value;
	}
}