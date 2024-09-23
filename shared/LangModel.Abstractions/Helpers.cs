using MathNet.Numerics.LinearAlgebra;

namespace LangModel.Abstractions;

public static class Helpers
{
    public static double CalculateSimilarity(
        this Vector<double> vec1,
        Vector<double> vec2)
    {
        var arr1 = vec1.ToArray();
        var arr2 = vec2.ToArray();

        if (arr1.Length != arr2.Length)
        {
            return 0;
        }

        double dotProduct = 0.0;
        double magnitude1 = 0.0;
        double magnitude2 = 0.0;

        for (int i = 0; i < arr1.Length; i++)
        {
            dotProduct += arr1[i] * arr2[i];
            magnitude1 += Math.Pow(arr1[i], 2);
            magnitude2 += Math.Pow(arr2[i], 2);
        }

        magnitude1 = Math.Sqrt(magnitude1);
        magnitude2 = Math.Sqrt(magnitude2);

        if (magnitude1 == 0.0 || magnitude2 == 0.0)
        {
            throw new ArgumentException(
                "Embedding must not have zero magnitude");
        }

        double cosineSimilarity = dotProduct / (magnitude1 * magnitude2);

        return cosineSimilarity;

        // Uncomment this if you need a cosin distance instead of similarity
        //double cosineDistance = 1 - cosineSimilarity;

        //return cosineDistance;
    }
}
