public interface IPause
{
    /// <summary>ポーズ時に実行</summary>
    public void OnPause();

    /// <summary>ポーズ時に解除</summary>
    public void OnResume();
}
