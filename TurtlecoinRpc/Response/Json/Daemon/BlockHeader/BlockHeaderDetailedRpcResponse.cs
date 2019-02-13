using Newtonsoft.Json;
using TurtlecoinRpc.Response.Json.Daemon.Transaction;

namespace TurtlecoinRpc.Response.Json.Daemon.BlockHeader
{
    public class BlockHeaderDetailedRpcResponse : BlockHeaderRpcResponse
    {
        /// <summary>
        /// Get the calculated median size from last 100 blocks.
        /// </summary>
        [JsonProperty("sizeMedian")]
        public ulong SizeMedian { get; set; }

        /// <summary>
        /// Get the fixed constant for max size of block.
        /// </summary>
        [JsonProperty("effectiveSizeMedian")]
        public ulong EffectiveSizeMedian { get; set; }

        /// <summary>
        /// Get the total sum of size of all transactions in the block.
        /// </summary>
        [JsonProperty("transactionsCumulativeSize")]
        public ulong TransactionsCumulativeSize { get; set; }

        /// <summary>
        /// Get the total number of coins generated in the network up to that block.
        /// </summary>
        [JsonProperty("alreadyGeneratedCoins")]
        public string AlreadyGeneratedCoins { get; set; }

        /// <summary>
        /// Get the total number of transactions present in the network up to that block.
        /// </summary>
        [JsonProperty("alreadyGeneratedTransactions")]
        public ulong AlreadyGeneratedTransactions { get; set; }

        /// <summary>
        /// Get the calculated reward.
        /// </summary>
        [JsonProperty("baseReward")]
        public ulong BaseReward { get; set; }

        /// <summary>
        /// Get the penalty in block reward determined for deviation.
        /// </summary>
        [JsonProperty("penalty")]
        public double Penalty { get; set; }

        /// <summary>
        /// Get the total fees for the transactions in the block.
        /// </summary>
        [JsonProperty("totalFeeAmount")]
        public ulong TotalFeeAmount { get; set; }

        /// <summary>
        /// Get the array of transactions in the block.
        /// </summary>
        [JsonProperty("transactions")]
        public TransactionShortRpcResponse[] Transactions { get; set; }
    }
}