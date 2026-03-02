namespace BlazorUI.Components.Motion;

/// <summary>
/// Extension methods and helpers for MotionEasing enum.
/// </summary>
public static class MotionEasingExtensions
{
    /// <summary>
    /// Gets the cubic-bezier values for a given easing function.
    /// Returns null for Elastic and Bounce easings (these should use keyframe-based animations).
    /// </summary>
    public static double[]? GetCubicBezier(this MotionEasing easing)
    {
        return easing switch
        {
            // Basic
            MotionEasing.Linear => new[] { 0.0, 0.0, 1.0, 1.0 },

            // Quadratic
            MotionEasing.QuadraticIn => new[] { 0.11, 0.0, 0.5, 0.0 },
            MotionEasing.QuadraticOut => new[] { 0.5, 1.0, 0.89, 1.0 },
            MotionEasing.QuadraticInOut => new[] { 0.45, 0.0, 0.55, 1.0 },

            // Cubic
            MotionEasing.CubicIn => new[] { 0.32, 0.0, 0.67, 0.0 },
            MotionEasing.CubicOut => new[] { 0.33, 1.0, 0.68, 1.0 },
            MotionEasing.CubicInOut => new[] { 0.65, 0.0, 0.35, 1.0 },

            // Quartic
            MotionEasing.QuarticIn => new[] { 0.5, 0.0, 0.75, 0.0 },
            MotionEasing.QuarticOut => new[] { 0.25, 1.0, 0.5, 1.0 },
            MotionEasing.QuarticInOut => new[] { 0.76, 0.0, 0.24, 1.0 },

            // Quintic
            MotionEasing.QuinticIn => new[] { 0.64, 0.0, 0.78, 0.0 },
            MotionEasing.QuinticOut => new[] { 0.22, 1.0, 0.36, 1.0 },
            MotionEasing.QuinticInOut => new[] { 0.83, 0.0, 0.17, 1.0 },

            // Sinusoidal
            MotionEasing.SinusoidalIn => new[] { 0.12, 0.0, 0.39, 0.0 },
            MotionEasing.SinusoidalOut => new[] { 0.61, 1.0, 0.88, 1.0 },
            MotionEasing.SinusoidalInOut => new[] { 0.37, 0.0, 0.63, 1.0 },

            // Exponential
            MotionEasing.ExponentialIn => new[] { 0.7, 0.0, 0.84, 0.0 },
            MotionEasing.ExponentialOut => new[] { 0.16, 1.0, 0.3, 1.0 },
            MotionEasing.ExponentialInOut => new[] { 0.87, 0.0, 0.13, 1.0 },

            // Circular
            MotionEasing.CircularIn => new[] { 0.55, 0.0, 1.0, 0.45 },
            MotionEasing.CircularOut => new[] { 0.0, 0.55, 0.45, 1.0 },
            MotionEasing.CircularInOut => new[] { 0.85, 0.0, 0.15, 1.0 },

            // Back (Overshoot)
            MotionEasing.BackIn => new[] { 0.36, 0.0, 0.66, -0.56 },
            MotionEasing.BackOut => new[] { 0.34, 1.56, 0.64, 1.0 },
            MotionEasing.BackInOut => new[] { 0.68, -0.6, 0.32, 1.6 },

            // Elastic and Bounce - these need keyframe-based animations, not cubic-bezier
            // Returning approximations with cubic-bezier (won't be perfect)
            MotionEasing.ElasticIn => null,
            MotionEasing.ElasticOut => null,
            MotionEasing.ElasticInOut => null,
            MotionEasing.BounceIn => null,
            MotionEasing.BounceOut => null,
            MotionEasing.BounceInOut => null,

            // Legacy aliases (map to quadratic)
            MotionEasing.EaseIn => new[] { 0.11, 0.0, 0.5, 0.0 },
            MotionEasing.EaseOut => new[] { 0.5, 1.0, 0.89, 1.0 },
            MotionEasing.EaseInOut => new[] { 0.45, 0.0, 0.55, 1.0 },

            // Custom - caller provides their own bezier
            MotionEasing.Custom => null,

            _ => new[] { 0.0, 0.0, 1.0, 1.0 } // Fallback to linear
        };
    }

    /// <summary>
    /// Converts the easing to a CSS cubic-bezier string.
    /// </summary>
    public static string ToCssString(this MotionEasing easing)
    {
        var bezier = easing.GetCubicBezier();
        if (bezier == null)
        {
            return easing switch
            {
                MotionEasing.Linear => "linear",
                MotionEasing.Custom => "linear", // Fallback if custom not provided
                _ => "linear" // Fallback for elastic/bounce if not using keyframes
            };
        }

        return $"cubic-bezier({bezier[0]:F2}, {bezier[1]:F2}, {bezier[2]:F2}, {bezier[3]:F2})";
    }

    /// <summary>
    /// Checks if this easing function requires keyframe-based animation
    /// (cannot be represented as cubic-bezier).
    /// </summary>
    public static bool RequiresKeyframes(this MotionEasing easing)
    {
        return easing is 
            MotionEasing.ElasticIn or 
            MotionEasing.ElasticOut or 
            MotionEasing.ElasticInOut or 
            MotionEasing.BounceIn or 
            MotionEasing.BounceOut or 
            MotionEasing.BounceInOut;
    }
}
