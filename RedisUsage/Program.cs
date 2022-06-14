using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace RedisUsage
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // 連線字串
            string address = "127.0.0.1:6379";
            // 建立連線
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(address);
            // 取得實體
            IDatabase db =  redis.GetDatabase();

            #region ExpiredTime
            // 過期時間
            var expiredTime = TimeSpan.FromSeconds(5);

            // 加入測試資料
            await db.StringSetAsync("sample", "hello world");

            // 設定過期時間
            var success = await db.KeyExpireAsync("sample", expiredTime); 
            #endregion

            #region HashSet
            // HashSet
            var hashKey = "hashData:Max";

            // 新增資料
            await db.HashSetAsync(hashKey, new HashEntry[]
            {
                new HashEntry ("Number","1"),
                new HashEntry ("Name","Max"),
                new HashEntry ("Phone","3345678")
            });

            await db.KeyExpireAsync(hashKey, expiredTime);

            // 查看指定資料的所有值
            var hashSetValues = await db.HashValuesAsync(hashKey);

            // 查看指定資料的所有 Key
            var hashSetKeys = await db.HashKeysAsync(hashKey); 
            #endregion

            #region ZSet
            // ZSet(SortedSet)
            var sortedSetKey = "sortedSet";

            // 新增一筆資料
            await db.SortedSetAddAsync(sortedSetKey, "Amy", 5);

            // 新增多筆資料
            await db.SortedSetAddAsync(sortedSetKey, new SortedSetEntry[]
            {
                new SortedSetEntry("Andy", 8),
                new SortedSetEntry("Danny", 4)
            });

            // 取得 SortedSet 長度
            var sortedSetLength = await db.SortedSetLengthAsync(sortedSetKey);

            // 取得 SortedSet 所有元素
            var sortedSetItems = await db.SortedSetRangeByValueAsync(sortedSetKey);

            // 取得 SortedSet 所有元素包含分數
            var sortedSetItemsAndSorce = await db.SortedSetRangeByScoreWithScoresAsync(sortedSetKey);

            // 取得分數遞減排序後 Danny 的排名，默認從索引 0 開始排
            var rankDanny = await db.SortedSetRankAsync(sortedSetKey, "Danny", Order.Descending);

            // 取得分數遞減前10名元素的排名，默認從索引 0 開始排
            var rankValues = await db.SortedSetRangeByRankAsync(sortedSetKey, start: 0, stop: 10, order: Order.Descending);

            // 取得分數遞減前10名元素的排名和分數，默認從索引 0 開始排
            var rankValueAndScore = await db.SortedSetRangeByScoreWithScoresAsync(sortedSetKey, start: 0, stop: 10, order: Order.Descending);

            // 刪除指定元素
            await db.SortedSetRemoveAsync(sortedSetKey, "Andy"); 
            #endregion

            #region Set
            // Set
            var setKey = "set";

            // 建立一筆資料
            await db.SetAddAsync(setKey, "1");

            // 建立多筆資料
            await db.SetAddAsync(setKey, new RedisValue[] { 1, 1, 2, 3 });

            // 刪除為 1 的資料
            await db.SetRemoveAsync(setKey, 1);

            // 取得 Set 長度
            var setLength = await db.SetLengthAsync(setKey);

            // 取得 Set 所有元素
            var setItems = await db.SetMembersAsync(setKey);

            // 取的指定元素
            var itemExists = await db.SetContainsAsync(setKey, 1);
            #endregion

            #region String
            // String
            // 設定資料
            var stringKey = "Hello";

            await db.StringSetAsync(stringKey, "0523");

            // 取得資料
            string value = await db.StringGetAsync(stringKey);

            Console.WriteLine($"Key {stringKey}，Value: {value}");

            // 刪除資料
            await db.KeyDeleteAsync(stringKey);  
            #endregion

            #region List
            // List
            var listkey = "List";
            // 從頭部加入元素
            await db.ListRightPushAsync(listkey, "AAA");

            // 從尾部加入元素
            await db.ListLeftPushAsync(listkey, "BBB");

            // 取出指定 Index 元素
            var indexValue = await db.ListGetByIndexAsync(listkey, 1);

            // 取出陣列
            var array = (await db.ListRangeAsync(listkey)).ToStringArray();

            // 取出子陣列，取 index 1 之後的 2 個
            var subArray = (await db.ListRangeAsync(listkey, 1, 2)).ToStringArray();

            // 插入多筆資料
            var insertCount = await db.ListRightPushAsync(listkey, new RedisValue[] { "B", "B", "B", "D" });

            // 刪除特定資料，將 A 的資料全部刪除
            await db.ListRemoveAsync(listkey, "A", 0);

            // 刪除特定資料，從頭開始刪除 1 筆 B 的資料
            await db.ListRemoveAsync(listkey, "B", 1);

            // 刪除特定資料，從尾開始刪除 2 筆 B 的資料
            await db.ListRemoveAsync(listkey, "B", -2);

            // 保留一段範圍的資料，取 index 1 之後的 2 個
            await db.ListTrimAsync(listkey, 1, 2); 
            #endregion
        }
    }
}
