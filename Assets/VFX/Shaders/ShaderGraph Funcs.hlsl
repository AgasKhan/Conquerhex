inline float unity_noise_randomValue(float2 uv)
{
    return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
}

inline float unity_noise_interpolate(float a, float b, float t)
{
    return (1.0 - t) * a + (t * b);
}

inline float unity_valueNoise(float2 uv)
{
    float2 i = floor(uv);
    float2 f = frac(uv);
    f = f * f * (3.0 - 2.0 * f);

    uv = abs(frac(uv) - 0.5);
    float2 c0 = i + float2(0.0, 0.0);
    float2 c1 = i + float2(1.0, 0.0);
    float2 c2 = i + float2(0.0, 1.0);
    float2 c3 = i + float2(1.0, 1.0);
    float r0 = unity_noise_randomValue(c0);
    float r1 = unity_noise_randomValue(c1);
    float r2 = unity_noise_randomValue(c2);
    float r3 = unity_noise_randomValue(c3);

    float bottomOfGrid = unity_noise_interpolate(r0, r1, f.x);
    float topOfGrid = unity_noise_interpolate(r2, r3, f.x);
    float t = unity_noise_interpolate(bottomOfGrid, topOfGrid, f.y);
    return t;
}

void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
{
    float t = 0.0;

    float freq = pow(2.0, float(0));
    float amp = pow(0.5, float(3 - 0));
    t += unity_valueNoise(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

    freq = pow(2.0, float(1));
    amp = pow(0.5, float(3 - 1));
    t += unity_valueNoise(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

    freq = pow(2.0, float(2));
    amp = pow(0.5, float(3 - 2));
    t += unity_valueNoise(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

    Out = t;
}

/////////////////////////////////////////////////////////

float2 unity_gradientNoise_dir(float2 p)
{
    p = p % 289;
    float x = (34 * p.x + 1) * p.x % 289 + p.y;
    x = (34 * x + 1) * x % 289;
    x = frac(x / 41) * 2 - 1;
    return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
}

float unity_gradientNoise(float2 p)
{
    float2 ip = floor(p);
    float2 fp = frac(p);
    float d00 = dot(unity_gradientNoise_dir(ip), fp);
    float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
    float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
    float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
    fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
    return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
}

void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
{
    Out = unity_gradientNoise(UV * Scale) + 0.5;
}

/////////////////////////////////////////////////////////

inline float2 unity_voronoi_noise_randomVector(float2 UV, float offset)
{
    float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
    UV = frac(sin(mul(UV, m)) * 46839.32);
    return float2(sin(UV.y * +offset) * 0.5 + 0.5, cos(UV.x * offset) * 0.5 + 0.5);
}

void Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
{
    float2 g = floor(UV * CellDensity);
    float2 f = frac(UV * CellDensity);
    float t = 8.0;
    float3 res = float3(8.0, 0.0, 0.0);

    for (int y = -1; y <= 1; y++)
    {
        for (int x = -1; x <= 1; x++)
        {
            float2 lattice = float2(x, y);
            float2 offset = unity_voronoi_noise_randomVector(lattice + g, AngleOffset);
            float d = distance(lattice + offset, f);
            if (d < res.x)
            {
                res = float3(d, offset.x, offset.y);
                Out = res.x;
                Cells = res.y;
            }
        }
    }
}

/////////////////////////////////////////////////////////

void Circle_float(float2 In, float2 center, float r, float distanceVar, out float2 Out)
{
    if (distance(In, center) < r)
        Out.x = 1;
    else
        Out.x = 0;
    
    Out.y = ((distanceVar / (distance(In, center) - r)));
    
    Out = saturate(Out);

}

void Manchota_float(float2 In, float2 center, float rng, float r, float distanceVar, out float Out)
{
    float2 Out2;
    
    Circle_float(In, center, r, distanceVar, Out2);
    
    float OutNoise;
    
    Unity_SimpleNoise_float(In + rng, 30, OutNoise);
    
    Out2.y *= saturate(OutNoise + 0.1);
    
    Out = Out2.x + step(0.2, Out2.y);
}


void PointPerpendicular_float(float2 In, float m, float a, float b, out float2 Out)
{
    Out = float2(In.x, b);

    if (m != 0)
    {
        float m2 = -1 / m;
        Out.x = In.y - m * a - b - m2 * In.x;
        Out.x /= m - m2;
        Out.y = m * (Out.x + a) + b;
    }
}


void Lineal_float(float2 In, float m, float a, float b, float diference, out float Out)
{
    float2 cercano;
    
    PointPerpendicular_float(In, m, a, b, cercano);

    if (distance(In, cercano) < diference)
        Out = 1;
    else
        Out = 0;
}





void LinealPoint_float(float2 In, float2 start, float2 end, float diference, out float Out)
{
    float maxX = end.x > start.x ? end.x : start.x;
    float minX = start.x > end.x ? end.x : start.x;
    float maxY = end.y > start.y ? end.y : start.y;
    float minY = start.y > end.y ? end.y : start.y;
    
    if (In.x > maxX + diference / 1.25 || In.x < minX - diference / 1.25 || In.y < minY - diference / 1.25 || In.y > maxY + diference / 1.25)
    {
        Out = 0;
        return;
    }
    
    if (start.x != end.x)
    {        
        float m = (end.y - start.y) / (end.x - start.x);
        float b = start.y - m * start.x;
        Lineal_float(In, m, (start.y - b) / m - start.x, b, diference, Out);
        return;
    }
    
    start.y = In.y;
    
    if (distance(In, start) < diference)
        Out = 1;
    else
        Out = 0;
}

void Bezier_float(float2 In, float2 start, float2 mid, float2 end, float diference, out float Out)
{    
    // Inicializamos el resultado en 0 (fuera de la curva)
    Out = 0.0;

    // Estimación inicial de t
    float t = 0.5;
    float minDistance = diference;

    [unroll] 
    for (int i = 0; i < 100; i++)
    {
        float oneMinusT = 1.0 - t;
        float2 B = oneMinusT * oneMinusT * start + 2.0 * oneMinusT * t * mid + t * t * end;
        float dist = distance(In, B);

        if (dist < minDistance)
        {
            Out = 1.0;
            return;
        }

        t += 0.01; 
        if (t > 1.0)
            t -= 1.0; // Reiniciar t si excede 1.0
    }
}

void Bezier2_float(float2 In, float2 start, float curvatura, float2 end, float diference, out float Out)
{
    Out = 0.0;
    float2 midpoint = 0.5 * (start + end);

    // Vector dirección de la línea de start a end
    float2 direction = end - start;
    float2 perpendicularDirection = normalize(float2(-direction.y, direction.x)); // Perpendicular a la línea

    // Punto de control, desplazado desde el punto medio en la dirección perpendicular
    float2 controlPoint = midpoint + perpendicularDirection * distance(start,end) * curvatura;
    
    
    Bezier_float(In, start, controlPoint, end, diference, Out);
}



void Path_float(float2 In, float2 start, float2 mid, float2 end, float diference, out float Out)
{
    float resultado;
    float resultado2;
    
    LinealPoint_float(In, start, mid, diference, resultado);
  
    
    LinealPoint_float(In, mid, end, diference, resultado2);
    
    Out = resultado + resultado2;
}
