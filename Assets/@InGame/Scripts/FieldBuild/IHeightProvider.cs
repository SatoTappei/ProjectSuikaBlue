/// <summary>
/// フィールド作成クラスから各セルに高さを渡すためのインターフェース
/// </summary>
public interface IHeightProvider
{
    /// <summary>
    /// 生成したパーリンノイズの高さが渡される
    /// </summary>
    void SetHeight(float height);
}
