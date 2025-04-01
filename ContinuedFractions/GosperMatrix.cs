using System.Numerics;
using System;
using System.Collections.Generic; // Для List в проверке знака
using System.Diagnostics.CodeAnalysis; // Для NotNullWhen

public class GosperMatrix
{
    public readonly BigInteger A, B, C, D;
    public readonly BigInteger E, F, G, H;

    public GosperMatrix(BigInteger a, BigInteger b, BigInteger c, BigInteger d,
                        BigInteger e, BigInteger f, BigInteger g, BigInteger h)
    {
        A = a; B = b; C = c; D = d;
        E = e; F = f; G = g; H = h;
    }

    // --- Методы IngestX, IngestY, Produce на BigInteger ---
    public GosperMatrix IngestX(int term)
    {
        BigInteger bigTerm = term;
        BigInteger nextA = A * bigTerm + C;
        BigInteger nextB = B * bigTerm + D;
        BigInteger nextC = A;
        BigInteger nextD = B;
        BigInteger nextE = E * bigTerm + G;
        BigInteger nextF = F * bigTerm + H;
        BigInteger nextG = E;
        BigInteger nextH = F;
        return new GosperMatrix(nextA, nextB, nextC, nextD, nextE, nextF, nextG, nextH);
    }

     public GosperMatrix IngestY(int term)
    {
        BigInteger bigTerm = term;
        BigInteger nextA = A * bigTerm + B;
        BigInteger nextB = A;
        BigInteger nextC = C * bigTerm + D;
        BigInteger nextD = C;
        BigInteger nextE = E * bigTerm + F;
        BigInteger nextF = E;
        BigInteger nextG = G * bigTerm + H;
        BigInteger nextH = G;
        return new GosperMatrix(nextA, nextB, nextC, nextD, nextE, nextF, nextG, nextH);
    }

    public GosperMatrix Produce(BigInteger q) // q теперь BigInteger
    {
        // Новый числитель - это старый знаменатель
        BigInteger nextA = E;
        BigInteger nextB = F;
        BigInteger nextC = G;
        BigInteger nextD = H;
        // Новый знаменатель = Старый числитель - q * Старый знаменатель
        BigInteger nextE = A - q * E;
        BigInteger nextF = B - q * F;
        BigInteger nextG = C - q * G;
        BigInteger nextH = D - q * H;
        return new GosperMatrix(nextA, nextB, nextC, nextD, nextE, nextF, nextG, nextH);
    }

    // --- Метод TryGetNextTerm (Пункт 4) ---

    /// <summary>
    /// Пытается определить следующий целый член q результирующей непрерывной дроби.
    /// Использует анализ предельных значений Z(x,y) = Num/Den при x,y >= 1
    /// через анализ сдвинутых коэффициентов (для xx=x-1>=0, yy=y-1>=0).
    /// </summary>
    /// <param name="q">Найденный следующий член (если метод возвращает true).</param>
    /// <returns>true, если следующий член q однозначно определен, иначе false.</returns>
    public bool TryGetNextTerm([NotNullWhen(true)] out BigInteger? q)
    {
        q = null;

        // 1. Вычисляем сдвинутые коэффициенты a', b', c', d', e', f', g', h'
        // Numerator': a'xx*yy + b'xx + c'yy + d'
        BigInteger ap = A;                     // a' = A
        BigInteger bp = A + B;                 // b' = A + B
        BigInteger cp = A + C;                 // c' = A + C
        BigInteger dp = A + B + C + D;         // d' = A + B + C + D

        // Denominator': e'xx*yy + f'xx + g'yy + h'
        BigInteger ep = E;                     // e' = E
        BigInteger fp = E + F;                 // f' = E + F
        BigInteger gp = E + G;                 // g' = E + G
        BigInteger hp = E + F + G + H;         // h' = E + F + G + H

        // 2. Проверка знаменателя Den' на возможную смену знака (наивный метод)
        if (ep == 0 && fp == 0 && gp == 0 && hp == 0)
        {
            // Знаменатель всегда 0.
            // Если числитель тоже всегда 0 (ap=bp=cp=dp=0), результат 0/0 - неопределенность.
            // Если числитель не всегда 0, результат - бесконечность.
            // В обоих случаях мы не можем выдать конечный q.
            return false;
        }

        // Собираем ненулевые коэффициенты знаменателя для проверки знака
        var denSigns = new List<int>();
        if (ep != 0) denSigns.Add(ep.Sign);
        if (fp != 0) denSigns.Add(fp.Sign);
        if (gp != 0) denSigns.Add(gp.Sign);
        if (hp != 0) denSigns.Add(hp.Sign);

        // Если есть и положительные, и отрицательные -> возможна смена знака -> нельзя определить q
        bool hasPositive = false;
        bool hasNegative = false;
        foreach (int sign in denSigns)
        {
            if (sign > 0) hasPositive = true;
            if (sign < 0) hasNegative = true;
        }
        if (hasPositive && hasNegative)
        {
            // Знаки смешанные, риск смены знака Den' -> не можем определить q
            return false;
        }
        // Если все ненулевые коэффициенты одного знака (или их нет, но hp != 0), то считаем, что знак Den' постоянен.

        // 3. Вычисляем предельные отношения и их floor
        BigInteger? minFloor = null;
        BigInteger? maxFloor = null;

        // Функция для безопасного вычисления floor(N/D) и обновления min/max
        void UpdateMinMaxFloor(BigInteger num, BigInteger den)
        {
            if (den == 0) return; // Пропускаем отношение, если знаменатель 0

            BigInteger currentFloor = FloorDiv(num, den);

            if (minFloor == null) // Первый вычисленный floor
            {
                minFloor = currentFloor;
                maxFloor = currentFloor;
            }
            else
            {
                if (currentFloor < minFloor) minFloor = currentFloor;
                if (currentFloor > maxFloor) maxFloor = currentFloor;
            }
        }

        UpdateMinMaxFloor(ap, ep); // Z(inf, inf) ~ a'/e'
        UpdateMinMaxFloor(bp, fp); // Z(inf, 1)   ~ b'/f'
        UpdateMinMaxFloor(cp, gp); // Z(1,   inf) ~ c'/g'
        UpdateMinMaxFloor(dp, hp); // Z(1,   1)   ~ d'/h'

        // 4. Проверка результата
        if (minFloor == null)
        {
            // Ни одного конечного отношения не вычислено (все знаменатели e',f',g',h' были 0)
            // Это должно было отсечься проверкой ep=fp=gp=hp=0 выше. Но на всякий случай.
             return false;
        }

        if (minFloor == maxFloor)
        {
            // Все конечные предельные значения имеют одинаковый floor - это наш q!
            q = minFloor;
            return true;
        }
        else
        {
            // floor(min Z) != floor(max Z) -> недостаточно информации, нужно запросить еще члены x или y
            return false;
        }
    }

    /// <summary>
    /// Вычисляет floor(n / d) для BigInteger.
    /// </summary>
    public static BigInteger FloorDiv(BigInteger n, BigInteger d)
    {
        if (d == 0) throw new DivideByZeroException("Attempted FloorDiv by zero.");

        // Проще всего работать с положительным делителем
        if (d < 0)
        {
            n = -n;
            d = -d;
        }

        BigInteger q = BigInteger.DivRem(n, d, out BigInteger r);

        // Если остаток отрицательный, результат деления (округление к нулю)
        // на 1 больше, чем floor. Нужно вычесть 1.
        // (n = q*d + r, где знак r совпадает со знаком n, если r!=0) -- НЕПРАВИЛЬНО для DivRem!
        // DivRem: n = q*d + r, где 0 <= |r| < |d|, знак r совпадает со знаком n.
        // Пример: FloorDiv(-5, 2). n=-5, d=2. q = -2, r = -1.
        //           Floor(-5/2) = Floor(-2.5) = -3. q= -2. r = -1.
        //           Нужно q-1 если r < 0.
        // Пример: FloorDiv(5, -2). n=5, d=-2 -> n=-5, d=2. q = -2, r = -1.
        //           Floor(5/-2) = Floor(-2.5) = -3. q=-2. r=-1.
        //           Нужно q-1 если r < 0.
        if (r < 0)
        {
            return q - 1;
        }

        return q;
    }

    // Начальные матрицы для операций:
    public static GosperMatrix Addition()
        => new GosperMatrix
            (
             0
           , 1
           , 1
           , 0
           , 0
           , 0
           , 0
           , 1
            ); // (0xy + 1x + 1y + 0) / (0xy + 0x + 0y + 1) = x + y

    public static GosperMatrix Subtraction()
        => new GosperMatrix
            (
             0
           , 1
           , -1
           , 0
           , 0
           , 0
           , 0
           , 1
            ); // (x - y) / 1

    public static GosperMatrix Multiplication()
        => new GosperMatrix
            (
             1
           , 0
           , 0
           , 0
           , 0
           , 0
           , 0
           , 1
            ); // (1xy + 0x + 0y + 0) / 1 = xy

    public static GosperMatrix Division()
        => new GosperMatrix
            (
             0
           , 1
           , 0
           , 0
           , 0
           , 0
           , 1
           , 0
            ); // (0xy + 1x + 0y + 0) / (0xy + 0x + 1y + 0) = x / y

    public override string ToString() { return $"({A}xy+{B}x+{C}y+{D})/({E}xy+{F}x+{G}y+{H})"; }
}
