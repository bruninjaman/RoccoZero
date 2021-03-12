namespace O9K.Core.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Divine.SDK.Extensions;

    using SharpDX;

    public class Polygon
    {
        public List<Vector2> Points = new List<Vector2>();

        public void Add(Vector2 point)
        {
            this.Points.Add(point);
        }

        public void Add(Vector3 point)
        {
            this.Points.Add(point.ToVector2());
        }

        public void Add(Polygon polygon)
        {
            foreach (var point in polygon.Points)
            {
                this.Points.Add(point);
            }
        }

        public virtual void Draw(Color color, int width = 1)
        {
            /*for (var i = 0; i <= (this.Points.Count - 1); i++)
            {
                var nextIndex = this.Points.Count - 1 == i ? 0 : i + 1;

                var fromTmp = new Vector3(this.Points[i].X, this.Points[i].Y, 0);
                var toTmp = new Vector3(this.Points[nextIndex].X, this.Points[nextIndex].Y, 0);

                var from = Drawing.WorldToScreen(fromTmp);
                var to = Drawing.WorldToScreen(toTmp);
                Drawing.DrawLine(from, to, color);
            }*/
        }

        public bool IsInside(Vector2 point)
        {
            return !this.IsOutside(point);
        }

        public bool IsInside(Vector3 point)
        {
            return !this.IsOutside(point.ToVector2());
        }

        public bool IsOutside(Vector2 point)
        {
            var p = new IntPoint(point.X, point.Y);
            return PointInPolygon(p, this.ToClipperPath()) != 1;
        }

        public List<IntPoint> ToClipperPath()
        {
            var result = new List<IntPoint>(this.Points.Count);
            result.AddRange(this.Points.Select(point => new IntPoint(point.X, point.Y)));
            return result;
        }

        public class Circle : Polygon
        {
            /// <summary>
            ///     The center
            /// </summary>
            public Vector2 Center;

            /// <summary>
            ///     The radius
            /// </summary>
            public float Radius;

            /// <summary>
            ///     The quality
            /// </summary>
            private readonly int _quality;

            /// <summary>
            ///     Initializes a new instance of the <see cref="Polygon.Circle" /> class.
            /// </summary>
            /// <param name="center">The center.</param>
            /// <param name="radius">The radius.</param>
            /// <param name="quality">The quality.</param>
            public Circle(Vector3 center, float radius, int quality = 20)
                : this(center.ToVector2(), radius, quality)
            {
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="Polygon.Circle" /> class.
            /// </summary>
            /// <param name="center">The center.</param>
            /// <param name="radius">The radius.</param>
            /// <param name="quality">The quality.</param>
            public Circle(Vector2 center, float radius, int quality = 20)
            {
                this.Center = center;
                this.Radius = radius;
                this._quality = quality;
                this.UpdatePolygon();
            }

            /// <summary>
            ///     Updates the polygon.
            /// </summary>
            /// <param name="offset">The offset.</param>
            /// <param name="overrideWidth">Width of the override.</param>
            public void UpdatePolygon(int offset = 0, float overrideWidth = -1)
            {
                this.Points.Clear();
                var outRadius = overrideWidth > 0 ? overrideWidth : (offset + this.Radius) / (float)Math.Cos((2 * Math.PI) / this._quality);
                for (var i = 1; i <= this._quality; i++)
                {
                    var angle = (i * 2 * Math.PI) / this._quality;
                    var point = new Vector2(this.Center.X + (outRadius * (float)Math.Cos(angle)), this.Center.Y + (outRadius * (float)Math.Sin(angle)));
                    this.Points.Add(point);
                }
            }
        }

        public class Rectangle : Polygon
        {
            public Vector2 End;

            public Vector2 Start;

            public float Width;

            public Rectangle(Vector3 start, Vector3 end, float width)
                : this(start.ToVector2(), end.ToVector2(), width)
            {
            }

            public Rectangle(Vector2 start, Vector2 end, float width)
            {
                this.Start = start;
                this.End = end;
                this.Width = width;
                this.UpdatePolygon();
            }

            public Vector2 Direction
            {
                get
                {
                    return (this.End - this.Start).Normalized();
                }
            }

            public Vector2 Perpendicular
            {
                get
                {
                    return this.Direction.Perpendicular();
                }
            }

            public void UpdatePolygon(int offset = 0, float overrideWidth = -1)
            {
                this.Points.Clear();
                this.Points.Add((this.Start + ((overrideWidth > 0 ? overrideWidth : this.Width + offset) * this.Perpendicular)) - (offset * this.Direction));
                this.Points.Add(this.Start - ((overrideWidth > 0 ? overrideWidth : this.Width + offset) * this.Perpendicular) - (offset * this.Direction));
                this.Points.Add((this.End - ((overrideWidth > 0 ? overrideWidth : this.Width + offset) * this.Perpendicular)) + (offset * this.Direction));
                this.Points.Add(this.End + ((overrideWidth > 0 ? overrideWidth : this.Width + offset) * this.Perpendicular) + (offset * this.Direction));
            }
        }

        public class Trapezoid : Polygon
        {
            public Vector2 End;

            public float endWidth;

            public Vector2 Start;

            public float startWidth;

            public Trapezoid(Vector3 start, Vector3 end, float startWidth, float endWidth)
                : this(start.ToVector2(), end.ToVector2(), startWidth, endWidth)
            {
            }

            public Trapezoid(Vector2 start, Vector2 end, float startWidth, float endWidth)
            {
                this.Start = start;
                this.End = end;
                this.endWidth = endWidth;
                this.startWidth = startWidth;
                this.UpdatePolygon();
            }

            public Vector2 Direction
            {
                get
                {
                    return (this.End - this.Start).Normalized();
                }
            }

            public Vector2 Perpendicular
            {
                get
                {
                    return this.Direction.Perpendicular();
                }
            }

            public void UpdatePolygon(int offset = 0, float overrideWidth = -1)
            {
                this.Points.Clear();
                this.Points.Add((this.Start + ((overrideWidth > 0 ? overrideWidth : this.startWidth + offset) * this.Perpendicular)) - (offset * this.Direction));
                this.Points.Add(this.Start - ((overrideWidth > 0 ? overrideWidth : this.startWidth + offset) * this.Perpendicular) - (offset * this.Direction));
                this.Points.Add((this.End - ((overrideWidth > 0 ? overrideWidth : this.endWidth + offset) * this.Perpendicular)) + (offset * this.Direction));
                this.Points.Add(this.End + ((overrideWidth > 0 ? overrideWidth : this.endWidth + offset) * this.Perpendicular) + (offset * this.Direction));
            }
        }

        public static int PointInPolygon(IntPoint pt, List<IntPoint> path)
        {
            int result = 0, cnt = path.Count;
            if (cnt < 3)
                return 0;
            IntPoint ip = path[0];
            for (int i = 1; i <= cnt; ++i)
            {
                IntPoint ipNext = (i == cnt ? path[0] : path[i]);
                if (ipNext.Y == pt.Y)
                {
                    if ((ipNext.X == pt.X) || (ip.Y == pt.Y &&
                      ((ipNext.X > pt.X) == (ip.X < pt.X))))
                        return -1;
                }
                if ((ip.Y < pt.Y) != (ipNext.Y < pt.Y))
                {
                    if (ip.X >= pt.X)
                    {
                        if (ipNext.X > pt.X)
                            result = 1 - result;
                        else
                        {
                            double d = (double)(ip.X - pt.X) * (ipNext.Y - pt.Y) -
                              (double)(ipNext.X - pt.X) * (ip.Y - pt.Y);
                            if (d == 0)
                                return -1;
                            else if ((d > 0) == (ipNext.Y > ip.Y))
                                result = 1 - result;
                        }
                    }
                    else
                    {
                        if (ipNext.X > pt.X)
                        {
                            double d = (double)(ip.X - pt.X) * (ipNext.Y - pt.Y) -
                              (double)(ipNext.X - pt.X) * (ip.Y - pt.Y);
                            if (d == 0)
                                return -1;
                            else if ((d > 0) == (ipNext.Y > ip.Y))
                                result = 1 - result;
                        }
                    }
                }
                ip = ipNext;
            }
            return result;
        }

        public struct IntPoint
        {
            public long X;
            public long Y;

            public long Z;

            public IntPoint(long x, long y, long z = 0)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
            }

            public IntPoint(double x, double y, double z = 0)
            {
                this.X = (long)x;
                this.Y = (long)y;
                this.Z = (long)z;
            }

            public IntPoint(IntPoint pt)
            {
                this.X = pt.X;
                this.Y = pt.Y;
                this.Z = pt.Z;
            }

            public static bool operator ==(IntPoint a, IntPoint b)
            {
                return a.X == b.X && a.Y == b.Y;
            }

            public static bool operator !=(IntPoint a, IntPoint b)
            {
                return a.X != b.X || a.Y != b.Y;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                if (obj is IntPoint)
                {
                    IntPoint a = (IntPoint)obj;
                    return (X == a.X) && (Y == a.Y);
                }
                else
                    return false;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

        }
    }
}