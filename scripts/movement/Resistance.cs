using System;
using System.Collections.Generic;

namespace PhysicsUtils
{
    public class Resistance
    {
        public float Value { get; private set; }
        public float Weight { get; private set; }

        public Resistance(float value, float weight = 0.5f)
        {
            if (value < 0 || value > 1)
            {
                throw new ArgumentException("Resistance value must be between 0 and 1");
            }

            Value = value;
            Weight = weight;
        }

        public static float CalculateNormalizedWeightedAverage(List<Resistance> frictions)
        {
            float sumOfProducts = 0;
            float sumOfWeights = 0;

            foreach (var friction in frictions)
            {
                sumOfProducts += friction.Value * friction.Weight;
                sumOfWeights += friction.Weight;
            }

            return sumOfWeights == 0 ? 0 : sumOfProducts / sumOfWeights;
        }
    }
}