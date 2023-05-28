using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI;
using Microsoft.Graphics.Canvas.Brushes;
using Windows.Foundation;

namespace ElectronBot.Braincase.ClockViews;

public class RainyDay
{

    #region Fileds
    private ICanvasImage img;
    private List<List<float>> presets;
    private List<Drop> drops;
    private CollisionMatrix matrix;
    private float gravity_force_factor_Y;
    private float gravity_force_factor_X;
    private bool enableSizeChange = true;
    private int fps = 60;
    private bool enableCollisions = true;
    private int gravityThreshold = 3;
    private float gravityAngle = (float)Math.PI / 2;
    private float gravityAngleVariance = 0;
    private float reflectionDropMappingWidth = 200;
    private float reflectionDropMappingHeight = 200;
    private float width;
    private float height;
    private float ImgScalefactor = 1;
    private Random random;
    private bool addDrop;
    private int speed;
    private DateTime lastExecutionTime;
    private Func<CanvasDrawingSession, Drop, bool> trail;
   // private Func<CanvasDrawingSession, Drop, bool> gravity;
    private Func<CanvasDrawingSession, Drop, DropItem, bool> collision;
    private Func<CanvasDrawingSession, Drop, bool> reflection;
    #endregion

    #region properties



    public float ImgSclaeFactor
    {
        get { return ImgScalefactor; }
        set { ImgScalefactor = value; }
    }

    public float Height
    {
        get { return height; }
        set { height = value; }
    }


    public float Width
    {
        get { return width; }
        set { width = value; }
    }

    public float GravityAngle
    {
        get { return gravityAngle; }
        set { gravityAngle = value; }
    }

    public bool EnableCollisions
    {
        get { return enableCollisions; }
        set { enableCollisions = value; }
    }


    public bool EnableSizeChange
    {
        get { return enableSizeChange; }
        set { enableSizeChange = value; }
    }


    public CollisionMatrix Matrix
    {
        get { return matrix; }
        set { matrix = value; }
    }
    public enum TrailType
    {
        Trail_None,
        Trail_Drops,
        Trail_Smudge
    }

    public enum GravityType
    {
        Gravity_None,
        Gravity_None_Linear,
        Gravity_Linear

    }

    public TrailType CurrentTrail { get; set; }

    public GravityType CurrentGravity { get; set; }
    public Func<CanvasDrawingSession, Drop, bool> Trail
    {
        get
        {
            switch (CurrentTrail)
            {
                case TrailType.Trail_None:
                     trail = Trail_None;
                    break;
                case TrailType.Trail_Drops:
                    trail = Trail_Drops;
                    break;
                case TrailType.Trail_Smudge:
                    trail = Trail_Smudge;
                    break;
                default:
                    trail = Trail_Drops;
                    break;
            }
            return trail;
        }
    }

    public Func<CanvasDrawingSession, Drop, bool> Gravity
    {
        get
        {
            Func<CanvasDrawingSession, Drop, bool> gravity;
            switch (CurrentGravity)
            {
                case GravityType.Gravity_None:
                    gravity = Gravity_None;
                    break;
                case GravityType.Gravity_None_Linear:
                    gravity = Gravity_None_Linear;
                    break;
                case GravityType.Gravity_Linear:
                    gravity = Gravity_Linear;
                    break;
                default:
                    gravity = Gravity_Linear;
                    break;
            }
            return gravity;
        }
    }

    public Func<CanvasDrawingSession, Drop, DropItem, bool> Collision
    {
        get { return collision; }
    }
    public Func<CanvasDrawingSession, Drop, bool> Reflection
    {
        get { return reflection; }
    }
    #endregion

    #region Constructor 

    public RainyDay(CanvasControl _canvas, float _width, float _height, ICanvasImage _img)
    {
        random = new Random();
        //prepare width and height of region
        width = _width;
        height = _height;
        //prepare imgReflected
        img = _img;
        //prepare drops
        drops = new List<Drop>();
        // assume defaults
        //  trail = Trail_Drops;
     
        CurrentGravity = GravityType.Gravity_Linear;
        collision = Collision_Sample;
        reflection = Reflection_Miniature;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Add a new raindrop to the  drops
    /// </summary>
    /// <param name="context"></param>
    private void AddDrop(CanvasDrawingSession context)
    {
        DateTime timestamp = DateTime.Now;
        if ((timestamp - lastExecutionTime).Milliseconds <= speed)
        {
            return;
        }
        lastExecutionTime = timestamp;
        if (drops.Count > 130) return;
        List<float> preset = new List<float>();
        for (var i = 0; i < presets.Count; i++)
        {
            if (presets[i][2] > 1 || presets[i][3] == -1)
            {
                if (presets[i][3] != 0)
                {
                    presets[i][3]--;
                    for (var y = 0; y < presets[i][2]; ++y)
                    {
                        PutDrop(context, new Drop((float)random.NextDouble() * width, (float)random.NextDouble() * height, presets[i][0], presets[i][1]));
                    }
                }
            }
            else if (random.NextDouble() < presets[i][2])
            {
                preset = presets[i];
                break;
            }
        }
        if (preset != null && preset.Count > 0)
        {
            PutDrop(context, new Drop((float)random.NextDouble() * width, (float)random.NextDouble() * height, preset[0], preset[1]));
        }

    }


    /// <summary>
    /// Adds a new raindrop to the animation.
    /// </summary>
    /// <param name="context">context</param>
    /// <param name="drop">drop object to be added to the animation</param>
    private void PutDrop(CanvasDrawingSession context, Drop drop)
    {
        drop.Draw(this, context);
        if (Gravity != null && drop.R > gravityThreshold)
        {
            if (enableCollisions)
            {
                matrix.Update(drop, false);
            }
            drops.Add(drop);
        }
    }

    /// <summary>
    /// Clear the drop and remove from the list if applicable.
    /// </summary>
    /// <param name="context">context</param>
    /// <param name="drop">drop to be cleared</param>
    /// <param name="force">force force removal from the list</param>
    /// <returns> result if true animation of this drop should be stopped</returns>
    private bool ClearDrop(CanvasDrawingSession context, Drop drop, bool force = false)
    {
        bool result = drop.Clear(this, context, force);
        if (result)
        {
            drops.Remove(drop);
        }
        return result;
    }

    #endregion

    #region Public Methods
    public void Rain(List<List<float>> _presets, int _speed)
    {
        speed = _speed;
        presets = _presets;

        gravity_force_factor_Y = (fps * 0.001f) / 25;
        gravity_force_factor_X = ((float)(Math.PI / 2) - gravityAngle) * (fps * 0.001f) / 50;

        if (enableCollisions)
        {
            // calculate max radius of a drop to establish gravity matrix resolution
            float maxDropRadius = 0;
            for (var i = 0; i < presets.Count; i++)
            {
                if (presets[i][0] + presets[i][1] > maxDropRadius)
                {
                    maxDropRadius = (float)Math.Floor((presets[i][0] + presets[i][1]));
                }
            }

            if (maxDropRadius > 0)
            {
                // initialize the gravity matrix
                int mwi = (int)Math.Ceiling(width / maxDropRadius);
                int mhi = (int)Math.Ceiling(height / maxDropRadius);
                matrix = new CollisionMatrix(mwi, mhi, maxDropRadius);
            }
            else
            {
                enableCollisions = false;
            }
        }
        for (var i = 0; i < presets.Count; i++)
        {
            if (presets[i].Count < 4)
            {
                presets[i].Add(-1);
            }
            else if (presets[i][3] == 0)
            {
                presets[i][3] = -1;
            }
        }
        lastExecutionTime = DateTime.Now;

    }


    public void UpdateDrops(CanvasDrawingSession context)
    {
        if (addDrop)
        {
            AddDrop(context);
        }
        List<Drop> dropsClone = new List<Drop>();
        drops.ForEach(item => { dropsClone.Add(item); });
        List<Drop> newDrops = new List<Drop>();
        int i;
        for (i = 0; i < dropsClone.Count; i++)
        {
            if (dropsClone[i].Animate(this, context))
            {
                newDrops.Add(dropsClone[i]);
            }
        }
        drops = newDrops;
        addDrop = true;
    }


    #region TRAIL function
    private bool Trail_None(CanvasDrawingSession context, Drop drop)
    {
        return true;
    }

    private bool Trail_Drops(CanvasDrawingSession context, Drop drop)
    {
        if (drop.TrailY == 0 || drop.Y - drop.TrailY >= random.NextDouble() * 100 * drop.R)
        {
            drop.TrailY = drop.Y;
            PutDrop(context, new Drop(drop.X + ((float)random.NextDouble() * 2 - 1) * (float)random.NextDouble(), drop.Y - drop.R - 5, (int)Math.Ceiling(drop.R / 5), 0));
        }
        return true;
    }

    private bool Trail_Smudge(CanvasDrawingSession context, Drop drop)
    {
        var y = drop.Y - drop.R - 3;
        var x = drop.X - drop.R / 2 + (random.NextDouble() * 2);
        if (y < 0 || x < 0)
        {
            return false;
        }
        context.DrawImage(img, new Rect(x, y, drop.R, 2), new Rect(x / ImgScalefactor, y / ImgScalefactor, drop.R / ImgScalefactor, 2 / ImgScalefactor), 1);

        return true;
    }
    #endregion

    #region Gravity Function
    private bool Gravity_None(CanvasDrawingSession context, Drop drop)
    {
        return true;
    }

    private bool Gravity_None_Linear(CanvasDrawingSession context, Drop drop)
    {
        if (ClearDrop(context, drop))
        {
            return true;
        }
        if (drop.Collided)
        {
            drop.Collided = false;
            drop.Seed = (float)Math.Floor(drop.R * ((float)random.NextDouble()) * fps);
            drop.Skipping = false;
            drop.Slowing = false;
        }
        else if (drop.Seed == 0 || drop.Seed < 0)
        {
            drop.Seed = (float)Math.Floor(drop.R * random.NextDouble() * fps);
            drop.Skipping = drop.Skipping == false ? true : false;
            drop.Slowing = true;
        }

        drop.Seed--;

        if (drop.Yspeed != 0)
        {
            if (drop.Slowing)
            {
                drop.Yspeed /= 1.1f;
                drop.Xspeed /= 1.1f;
                if (drop.Yspeed < gravity_force_factor_Y)
                {
                    drop.Slowing = false;
                }

            }
            else if (drop.Skipping)
            {
                drop.Yspeed = gravity_force_factor_Y;
                drop.Xspeed = gravity_force_factor_X;
            }
            else
            {
                drop.Yspeed += 1 * gravity_force_factor_Y * (float)Math.Floor(drop.R);
                drop.Xspeed += 1 * gravity_force_factor_X * (float)Math.Floor(drop.R);
            }
        }
        else
        {
            drop.Yspeed = gravity_force_factor_Y;
            drop.Xspeed = gravity_force_factor_X;
        }

        if (gravityAngleVariance != 0)
        {
            drop.Xspeed += ((float)(random.NextDouble() * 2 - 1) * drop.Yspeed * gravityAngleVariance);
        }

        drop.Y += drop.Yspeed;
        drop.X += drop.Xspeed;

        drop.Draw(this, context);
        return false;
    }

    private bool Gravity_Linear(CanvasDrawingSession context, Drop drop)
    {
        if (ClearDrop(context, drop))
        {
            return true;
        }

        if (drop.Yspeed != 0)
        {
            drop.Yspeed += gravity_force_factor_Y * (float)Math.Floor(drop.R);
            drop.Xspeed += gravity_force_factor_X * (float)Math.Floor(drop.R);
        }
        else
        {
            drop.Yspeed = gravity_force_factor_Y;
            drop.Xspeed = gravity_force_factor_X;
        }
        drop.X += drop.Xspeed;
        drop.Y += drop.Yspeed;
        drop.Draw(this, context);
        return false;
    }
    #endregion

    #region  COLLISION function
    private bool Collision_Sample(CanvasDrawingSession context, Drop drop, DropItem collisions)
    {
        DropItem item = collisions;
        Drop drop2 = null;
        while (item != null)
        {
            Drop p = item.Dropdata;

            if (Math.Sqrt(Math.Pow(drop.X - p.X, 2) + Math.Pow(drop.Y - p.Y, 2)) < (drop.R + p.R))
            {
                drop2 = p;

                break;
            }
            item = item.Next;
        }
        if (drop2 == null)
        {
            return false;
        }

        // rename so that we're dealing with low/high drops
        Drop higher, lower;
        if (drop.Y > drop2.Y)
        {
            higher = drop;
            lower = drop2;
        }
        else
        {
            higher = drop2;
            lower = drop;
        }
        // force stopping the second drop
        ClearDrop(context, lower);
        ClearDrop(context, higher, true);
        matrix.Remove(higher);
        lower.Draw(this, context);
        lower.Colliding = higher;
        lower.Collided = true;
        return true;
    }
    #endregion

    #region Reflection function
    private bool Reflection_Miniature(CanvasDrawingSession context, Drop drop)
    {
        var sx = Math.Max((drop.X / ImgScalefactor - reflectionDropMappingWidth), 0);
        var sy = Math.Max((drop.Y / ImgScalefactor - reflectionDropMappingHeight), 0);
        var sw = PositiveMin(reflectionDropMappingWidth * 2, width/ImgScalefactor - sx);
        var sh = PositiveMin(reflectionDropMappingHeight * 2, height / ImgScalefactor - sy);
        var dx = Math.Max(drop.X - 1.1 * drop.R, 0);
        var dy = Math.Max(drop.Y - 1.1 * drop.R, 0);
        context.DrawImage(img, new Rect(dx, dy, drop.R * 2, drop.R * 2), new Rect(sx, sy, sw, sh));
        return true;
    }
    private float PositiveMin(float val1, float val2)
    {
        float result = 0;
        if (val1 < val2)
        {
            if (val1 <= 0)
            {
                result = val2;
            }
            else
            {
                result = val1;
            }
        }
        else
        {
            if (val2 <= 0)
            {
                result = val1;
            }
            else
            {
                result = val2;
            }
        }
        result = result <= 0 ? 1 : result;
        return result;
    }
    #endregion



    #endregion

}
