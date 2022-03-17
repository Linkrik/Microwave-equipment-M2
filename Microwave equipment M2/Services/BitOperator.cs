
namespace Microwave_equipment_M2.Services
{
    public static class BitOperator
    {
        static uint[] cleaner;
        static uint[] bits;

        static BitOperator()
        {
            cleaner = new uint[32];
            for (int i = 1; i < 32; i++)
                cleaner[i - 1] = ((uint)1 << i) - 1;
            cleaner[31] = uint.MaxValue;

            bits = new uint[32];
            for (int i = 0; i < 32; i++)
            {
                bits[i] = (uint)(1 << i);
            }
        }

        public static uint SubstituteNumber(uint word, uint num, int startBit, int bitCount)
        {
            return (word & (~(cleaner[bitCount - 1] << startBit))) | (num << startBit);
        }

        public static uint ExtractNumber(uint word, int startBit, int bitCount)
        {
            return (word & (cleaner[bitCount - 1] << startBit)) >> startBit;
        }

        public static bool BitSetted(uint word, int bitNum)
        {
            return (word & bits[bitNum]) == bits[bitNum];
        }

        public static uint SubstituteZero(uint word, int startBit, int bitCount)
        {
            if (bitCount == 0)
                return word;
            return word & (~(cleaner[bitCount - 1] << startBit));
        }

        public static uint SubstituteOnes(uint word, int startBit, int bitCount)
        {
            return word | (cleaner[bitCount - 1] << startBit);
        }
    }
}