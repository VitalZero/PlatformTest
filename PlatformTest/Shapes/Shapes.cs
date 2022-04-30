using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VZShapes
{
    public sealed class Shapes : IDisposable
    {
        public static readonly float MinLinethickness = 1f;
        public static readonly float MaxLinethickness = 10f;

        private Game game;
        private BasicEffect effect;

        private VertexPositionColor[] vertices;
        private int[] indices;

        private int shapeCount;
        private int vertexCount;
        private int indexCount;

        private bool isStarted;

        private bool isDisposed;

        public Shapes(Game game)
        {
            this.game = game ?? throw new ArgumentException("game is not valid (null)");

            const int MaxVertexCount = 1024;
            const int MaxIndexCount = MaxVertexCount * 3;

            vertices = new VertexPositionColor[MaxVertexCount];
            indices = new int[MaxIndexCount];

            shapeCount = 0;
            vertexCount = 0;
            indexCount = 0;

            isStarted = false;
            isDisposed = false;

            effect = new BasicEffect(this.game.GraphicsDevice);
            effect.TextureEnabled = false;
            effect.LightingEnabled = false;
            effect.FogEnabled = false;
            effect.VertexColorEnabled = true;
            effect.World = Matrix.Identity;
            effect.View = Matrix.Identity;
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            effect?.Dispose();

            isDisposed = true;
        }

        public void Begin()
        {
            if(isStarted)
            {
                throw new Exception("batch already started");
            }

            effect.Projection = Matrix.CreateOrthographicOffCenter(
                0, 
                game.GraphicsDevice.Viewport.Width,
                game.GraphicsDevice.Viewport.Height,
                0f,
                0f, 
                1f);

            isStarted = true;
        }

        public void End()
        {

            Flush();

            isStarted = false;
        }

        public void Flush()
        {
            if (shapeCount == 0)
                return;

            EnsureStarted();

            foreach(EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleList,
                    vertices,
                    0,
                    vertexCount,
                    indices,
                    0,
                    indexCount / 3);
            }

            shapeCount = 0;
            vertexCount = 0;
            indexCount = 0;
        }

        private void EnsureStarted()
        {
            if (!isStarted)
            {
                throw new Exception("batch did not start");
            }
        }

        private void EnsureSpace(int shapeVertexCount, int shapeIndexCount)
        {
            if(shapeVertexCount > vertices.Length)
            {
                throw new Exception("Maximum shape vertex count is: " + vertices.Length);
            }

            if (shapeIndexCount > indices.Length)
            {
                throw new Exception("Maximum shape index count is: " + indices.Length);
            }

            if (vertexCount + shapeVertexCount > vertices.Length ||
                indexCount + shapeIndexCount > indices.Length)
            {
                Flush();
            }
        }

        public void DrawRectangleFill(float x, float y, float width, float height, Color color)
        {
            EnsureStarted();

            const int shapeVertexCount = 4;
            const int shapeIndexCount = 6;

            EnsureSpace(shapeVertexCount, shapeIndexCount);

            float left = x;
            float right = x + width;
            float top = y;
            float bottom = y + height;

            Vector2 a = new Vector2(left, top);
            Vector2 b = new Vector2(right, top);
            Vector2 c = new Vector2(right, bottom);
            Vector2 d = new Vector2(left, bottom);

            indices[indexCount++] = 0 + vertexCount;
            indices[indexCount++] = 1 + vertexCount;
            indices[indexCount++] = 2 + vertexCount;
            indices[indexCount++] = 0 + vertexCount;
            indices[indexCount++] = 2 + vertexCount;
            indices[indexCount++] = 3 + vertexCount;

            vertices[vertexCount++] = new VertexPositionColor(new Vector3(a, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(b, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(c, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(d, 0f), color);

            shapeCount++;
        }

        public void DrawRectangle(float x, float y, float width, float height, float thickness, Color color)
        {
            float left = x;
            float right = x + width;
            float top = y;
            float bottom = y + height;

            DrawLine(left, top, right, top, thickness, color);
            DrawLine(right, top, right, bottom, thickness, color);
            DrawLine(right, bottom, left, bottom, thickness, color);
            DrawLine(left, bottom, left, top, thickness, color);
        }

        public void DrawLineSlow(Vector2 start, Vector2 end, float thickness, Color color)
        {
            EnsureStarted();

            const int shapeVertexCount = 4;
            const int shapeIndexCount = 6;

            EnsureSpace(shapeVertexCount, shapeIndexCount);

            thickness = Math.Clamp(thickness, Shapes.MinLinethickness, Shapes.MaxLinethickness);

            float halfThickness = thickness / 2f;

            Vector2 e1 = end - start;
            e1.Normalize();
            e1 *= halfThickness; 
            Vector2 e2 = -e1;

            Vector2 n1 = new Vector2(-e1.Y, e1.X);
            Vector2 n2 = -n1;

            Vector2 q1 = start + n1 + e2;
            Vector2 q2 = end + n1 + e1;
            Vector2 q3 = end + n2 + e1;
            Vector2 q4 = start + n2 + e2;

            indices[indexCount++] = 2 + vertexCount;
            indices[indexCount++] = 0 + vertexCount;
            indices[indexCount++] = 3 + vertexCount;
            indices[indexCount++] = 2 + vertexCount;
            indices[indexCount++] = 1 + vertexCount;
            indices[indexCount++] = 0 + vertexCount;

            vertices[vertexCount++] = new VertexPositionColor(new Vector3(q1, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(q2, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(q3, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(q4, 0f), color);

            shapeCount++;
        }

        public void DrawLine(float x1, float y1, float x2, float y2, float thickness, Color color)
        {
            DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), thickness, color);
        }

        public void DrawLine(Vector2 start, Vector2 end, float thickness, Color color)
        {
            EnsureStarted();

            const int shapeVertexCount = 4;
            const int shapeIndexCount = 6;

            EnsureSpace(shapeVertexCount, shapeIndexCount);

            thickness = Math.Clamp(thickness, Shapes.MinLinethickness, Shapes.MaxLinethickness);

            float halfThickness = thickness / 2f;

            float e1x = end.X - start.X;
            float e1y = end.Y - start.Y;

            float invLen = 1f / (float)Math.Sqrt(e1x * e1x + e1y * e1y);

            e1x *= invLen; 
            e1y *= invLen;  

            e1x *= halfThickness;
            e1y *= halfThickness;

            float e2x = -e1x;
            float e2y = -e1y;

            float n1x = -e1y;
            float n1y = e1x;

            float n2x = -n1x;
            float n2y = -n1y;

            float q1x = start.X + n1x + e2x;
            float q1y = start.Y + n1y + e2y;

            float q2x = end.X + n1x + e1x;
            float q2y = end.Y + n1y + e1y;

            float q3x = end.X + n2x + e1x;
            float q3y = end.Y + n2y + e1y;

            float q4x = start.X + n2x + e2x;
            float q4y = start.Y + n2y + e2y;

            indices[indexCount++] = 2 + vertexCount;
            indices[indexCount++] = 0 + vertexCount;
            indices[indexCount++] = 3 + vertexCount;
            indices[indexCount++] = 2 + vertexCount;
            indices[indexCount++] = 1 + vertexCount;
            indices[indexCount++] = 0 + vertexCount;

            vertices[vertexCount++] = new VertexPositionColor(new Vector3(q1x, q1y, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(q2x, q2y, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(q3x, q3y, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(q4x, q4y, 0f), color);

            shapeCount++;
        }

        public void DrawCircle(float x, float y, float radius, int points, float thickness, Color color)
        {
            const int minPoints = 3;
            const int maxPoints = 256;

            points = Math.Clamp(points, minPoints, maxPoints);

            float deltaAngle = MathHelper.TwoPi / (float)points;

            float angle = 0f;

            for(int i = 0; i < points; ++i)
            {
                float ax = (float)Math.Sin(angle) * radius + x;
                float ay = (float)Math.Cos(angle) * radius + y;

                angle += deltaAngle;

                float bx = (float)Math.Sin(angle) * radius + x;
                float by = (float)Math.Cos(angle) * radius + y;

                DrawLine(ax, ay, bx, by, thickness, color);
            }
        }

        public void DrawPolygon(Vector2[] vertices, float thickness, Color color)
        {
            for (int i = 0; i < vertices.Length; ++i)
            {
                DrawLine(vertices[i], vertices[(i + 1) % vertices.Length], thickness, color);
            }
        }
    }
}
