using System;
using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.UI;

namespace ElectronBot.Braincase.ClockViews;

public class Drop
{
    #region Fileds
    private static CanvasGeometry clipGeo;
    private string gid = null;
    private int gmx;
    private int gmy;
    private float x;
    private float y;
    private float r;
    private Drop colliding;
    private bool collided;
    private bool slowing;
    private float yspeed;
    private float xspeed;
    private bool terminate;
    private float trailY;
    private float seed;
    private bool skipping;
    #endregion

    #region Properties
    public bool Skipping
    {
        get
        {
            return skipping;
        }
        set
        {
            skipping = value;
        }
    }

    public float Seed
    {
        get
        {
            return seed;
        }
        set
        {
            seed = value;
        }
    }
    public int Gmy
    {
        get
        {
            return gmy;
        }
        set
        {
            gmy = value;
        }
    }


    public int Gmx
    {
        get
        {
            return gmx;
        }
        set
        {
            gmx = value;
        }
    }

    public string Gid
    {
        get
        {
            return gid;
        }
        set
        {
            gid = value;
        }
    }
    public float X
    {
        get
        {
            return x;
        }
        set
        {
            x = value;
        }
    }

    public float Y
    {
        get
        {
            return y;
        }
        set
        {
            y = value;
        }
    }

    public float R
    {
        get
        {
            return r;
        }
        set
        {
            r = value;
        }
    }
    public float TrailY
    {
        get
        {
            return trailY;
        }
        set
        {
            trailY = value;
        }
    }

    public float Xspeed
    {
        get
        {
            return xspeed;
        }
        set
        {
            xspeed = value;
        }
    }

    public float Yspeed
    {
        get
        {
            return yspeed;
        }
        set
        {
            yspeed = value;
        }
    }
    public bool Collided
    {
        get
        {
            return collided;
        }
        set
        {
            collided = value;
        }
    }

    public Drop Colliding
    {
        get
        {
            return colliding;
        }
        set
        {
            colliding = value;
        }
    }

    public bool Slowing
    {
        get
        {
            return slowing;
        }
        set
        {
            slowing = value;
        }
    }


    #endregion

    #region Construtors
    public Drop(float centerX, float centerY, float min, float thisbase)
    {
        x = (float)Math.Floor(centerX);
        y = (float)Math.Floor(centerY);
        r = (float)((new Random()).NextDouble() * thisbase) + min;
    }


    #endregion

    #region public methods
    /// <summary>
    /// Draws a raindrop on canvas at the current position.
    /// </summary>
    public void Draw(RainyDay rainyday, CanvasDrawingSession context)
    {
        float orgR = r;
        r = 0.95f * r;
        if (r < 3)
        {
            clipGeo = CanvasGeometry.CreateCircle(context, new Vector2(x, y), r);
        }
        else if (colliding != null || yspeed > 2)
        {
            if (colliding != null)
            {
                var collider = colliding;
                r = 1.001f * (r > collider.r ? r : collider.r);
                x += (collider.x - x);
                colliding = null;
            }
            float yr = 1 + 0.1f * yspeed;
            using (CanvasPathBuilder path = new CanvasPathBuilder(context))
            {
                path.BeginFigure(x - r / yr, y);
                path.AddCubicBezier(new Vector2(x - r, y - r * 2), new Vector2(x + r, y - r * 2), new Vector2(x + r / yr, y));
                path.AddCubicBezier(new Vector2(x + r, y + yr * r), new Vector2(x - r, y + yr * r), new Vector2(x - r / yr, y));
                path.EndFigure(CanvasFigureLoop.Closed);
                clipGeo = CanvasGeometry.CreatePath(path);
            }
        }
        else
        {
            clipGeo = CanvasGeometry.CreateCircle(context, new Vector2(x, y), 0.9f * r);
        }
        r = orgR;
        if (rainyday.Reflection != null)
        {
            using (context.CreateLayer(1, clipGeo))
            {
                rainyday.Reflection(context, this);
            }
        }
        if (clipGeo != null)
        {
            clipGeo.Dispose();
        }
    }

    /// <summary>
    /// Clears the raindrop region.
    /// </summary>
    /// <param name="force">param force force stop</param>
    /// <returns>returns Boolean true if the animation is stopped</returns>
    public bool Clear(RainyDay rainyday, CanvasDrawingSession context, bool force = false)
    {
        context.Blend = CanvasBlend.Copy;
        context.FillRectangle(x - r - 1, y - r - 2, 2 * r + 2, 2 * r + 2, Colors.Transparent);
        context.Blend = CanvasBlend.SourceOver;
        if (force)
        {
            terminate = true;
            return true;
        }
        if (rainyday == null)
        {
            return true;
        }
        if ((y - r > rainyday.Height) || (x - r > rainyday.Width) || (x + r < 0))
        {
            // over edge so stop this drop
            return true;
        }
        return false;
    }

    /// <summary>
    /// Moves the raindrop to a new position according to the gravity.
    /// </summary>
    public bool Animate(RainyDay rainyday, CanvasDrawingSession context)
    {
        if (terminate)
        {
            return false;
        }
        var stopped = rainyday.Gravity(context, this);
        if (!stopped)
        {
            rainyday.Trail(context, this);
        }
        if (rainyday.EnableCollisions)
        {
            var collisions = rainyday.Matrix.Update(this, stopped);
            if (collisions != null)
            {
                rainyday.Collision(context, this, collisions);
            }
        }
        return !stopped || terminate;

    }
    #endregion

}
