using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculus
{
    // Finds neighboring positions.
    // If diagonal is 'true', finds all positions, else only the orthogonal positions.
    public static List<Vector2> FindNeighbors(int posX, int posY, bool diagonal)
    {
        List<Vector2> result = new List<Vector2>();

        result.Add(new Vector2(posX - 1, posY));
        result.Add(new Vector2(posX, posY - 1));
        result.Add(new Vector2(posX + 1, posY));
        result.Add(new Vector2(posX, posY + 1));

        if (diagonal)
        {
            result.Add(new Vector2(posX - 1, posY - 1));
            result.Add(new Vector2(posX + 1, posY - 1));
            result.Add(new Vector2(posX + 1, posY + 1));
            result.Add(new Vector2(posX - 1, posY + 1));
        }

        return result;
    }

    // Finds all L positions (chess Knight possible moves) around a point.
    public static List<Vector2> FindLPositions(int posX, int posY)
    {
        List<Vector2> result = new List<Vector2>();
        
        result.Add(new Vector2(posX + 1, posY + 2));
        result.Add(new Vector2(posX - 1, posY + 2));
        result.Add(new Vector2(posX + 2, posY + 1));
        result.Add(new Vector2(posX + 2, posY - 1));
        result.Add(new Vector2(posX + 1, posY - 2));
        result.Add(new Vector2(posX - 1, posY - 2));
        result.Add(new Vector2(posX - 2, posY + 1));
        result.Add(new Vector2(posX - 2, posY - 1));

        return result;
    }

    // Finds all coordinates of a line starting from a given point.
    // posX, posY - the starting point.
    // incrementX, incrementY - direction of the line. Can be -1, 0 or 1 (they can't be both '0'). If one of them is '0' then the line is orthogonal, else the line is diagonal.
    // min, max - limits (board borders).
    public static List<Vector2> FindLine(int posX, int posY, int incrementX, int incrementY, int min, int max, int length = 0)
    {
        List<Vector2> result = new List<Vector2>();

        // If both increments are 0 then there is no increment and no line.
        if (incrementX != 0 || incrementY != 0)
        {
            posX += incrementX;
            posY += incrementY;
            bool hasLength = length != 0;
            while (posX >= min && posY >= min && posX < max && posY < max)
            {
                if (hasLength)
                {
                    if (length != 0)
                    {
                        length--;
                    }
                    else
                    {
                        break;
                    }
                }
                result.Add(new Vector2(posX, posY));
                posX += incrementX;
                posY += incrementY;
            }
        }

        return result;
    }

    public static float FindLengthPythagorean(float a, float b)
    {
        return Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));
    }

}
