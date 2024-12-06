using System.Collections.Generic;
using osuTK;
using AnimationParser.Core;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using System;
using System.Linq;
using osuTK.Graphics;

namespace AnimationParser.Game;

public partial class AnimationScene : Container
{
    private class InterpreterContext : AnimationContext
    {
        // Containing Drawables that interacts with the Game Engine
        // In this case, osu!framework. But can be easily to adapt to other engines if you implement this context
        private Dictionary<string, Drawable> drawables = new();

        private Container container;
        public InterpreterContext(Container container)
        {
            this.container = container;
        }

        const int duration = 500;

        protected override void OnObjectAdded(string name, AnimationObject obj)
        {
            base.OnObjectAdded(name, obj);

            // Add a associated binding GameObject to the Game Engine
            var drawable = new Container()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.CentreLeft,
            };

            foreach (var shapeBase in obj.Shapes)
            {
                switch (shapeBase.Type)
                {
                    case ShapeType.Line:
                        {
                            var shape = (LineShape)shapeBase;
                            var vector = shape.End - shape.Start;
                            var length = vector.Length();

                            var rotation = MathHelper.RadiansToDegrees((float)Math.Atan2(vector.Y, vector.X));
                            var position = new Vector2(shape.Start.X, shape.Start.Y);

                            drawable.Add(new Box()
                            {
                                RelativeSizeAxes = Axes.None,
                                RelativePositionAxes = Axes.None,
                                Origin = Anchor.CentreLeft,
                                Height = 3.0f,
                                Width = length,
                                Rotation = rotation,
                                Position = position,
                            });
                            break;
                        }

                    case ShapeType.Circle:
                        {
                            var shape = (CircleShape)shapeBase;
                            drawable.Add(new Circle()
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Size = new Vector2(shape.Radius * 2),
                                Position = new Vector2(shape.Center.X, shape.Center.Y), // We have to cast to OpenTk.Vector2
                            });
                        }
                        break;
                }
            }

            drawables.Add(name, drawable);
            container.Add(drawable);

            drawable.FlashColour(Color4.Pink, 800, Easing.OutQuint);
        }

        protected override void OnObjectErasingOut(string name, AnimationObject obj)
        {
            if (drawables.TryGetValue(name, out var drawable))
            {
                var delay = drawable.Transforms.Select(t => t.EndTime - drawable.Time.Current)
                    .OrderDescending()
                    .FirstOrDefault();

                drawable.Delay(delay).Then().FadeOut(duration).Expire()
                    .OnComplete(_ =>
                    {
                        drawables.Remove(name);
                        base.OnObjectErasingOut(name, obj);
                    });
            }
        }

        protected override void OnObjectPlaced(string name, AnimationObject obj, System.Numerics.Vector2 position)
        {
            base.OnObjectPlaced(name, obj, position);

            if (drawables.TryGetValue(name, out var drawable))
            {
                var delay = drawable.Transforms.Select(t => t.EndTime - drawable.Time.Current)
                    .OrderDescending()
                    .FirstOrDefault();

                drawable.Delay(delay).Then().MoveTo(new Vector2(position.X, position.Y), duration);
            }
        }

        protected override void OnObjectShifting(string name, AnimationObject obj, Core.Direction direction)
        {
            base.OnObjectShifting(name, obj, direction);
            var rect = container.DrawRectangle;

            if (drawables.TryGetValue(name, out var drawable))
            {
                var delay = drawable.Transforms.Select(t => t.EndTime - drawable.Time.Current)
                    .OrderDescending()
                    .FirstOrDefault();

                switch (direction)
                {
                    case Core.Direction.Up:
                        drawable.Delay(delay).Then().MoveToY(-container.DrawHeight / 2, duration);
                        break;
                    case Core.Direction.Down:
                        drawable.Delay(delay).Then().MoveToY(container.DrawHeight / 2, duration);
                        break;
                    case Core.Direction.Left:
                        drawable.Delay(delay).Then().MoveToX(-container.DrawWidth / 2, duration);
                        break;
                    case Core.Direction.Right:
                        drawable.Delay(delay).Then().MoveToX(container.DrawWidth / 2, duration);
                        break;
                }
            }
        }
    }

    private InterpreterContext context;

    public AnimationScene()
    {
        context = new InterpreterContext(this);
        RelativeSizeAxes = Axes.Both;
    }

    public new void Clear()
    {
        base.Clear();
        context = new InterpreterContext(this);
    }

    public void ExecuteAnimationCommands(IEnumerable<IAnimationCommand> commands)
    {
        Clear();

        foreach (var command in commands)
        {
            command.Execute(context);
        }
    }
}
