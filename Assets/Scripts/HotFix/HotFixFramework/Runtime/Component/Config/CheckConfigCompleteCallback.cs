namespace Deer
{
    /// <summary>
    /// 使用可更新模式并检查资源Config完成时的回调函数。
    /// </summary>
    /// <param name="removedCount">已移除的资源数量。</param>
    /// <param name="updateCount">可更新的资源数量。</param>
    /// <param name="updateTotalLength">可更新的资源总大小。</param>
    public delegate void CheckConfigCompleteCallback(int movedCount,int removedCount, int updateCount, long updateTotalLength);
}