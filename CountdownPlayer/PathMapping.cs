using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountdownPlayer
{
    public interface IPathStep
    {
        int FrameSize { get; }
        int Index1 { get; }
        int Index2 { get; }
        int Operation { get; }
    }


    public class PathStep : IPathStep
    {

        private static int _op_count = -1;
        private static int op_count
        {
            get
            {
                if (_op_count == -1)
                    _op_count = CountdownPlayer.Operation.Operations().Count();

                return _op_count;
            }
        }
        public int FrameSize { get; set; }
        public int Index1 { get; set; }
        public int Index2 { get; set; }
        public int Operation { get; set; }
        public PathStep Parent { get; set; }

        public PathStep(int frame_size, PathStep parent = null)
        {
            this.FrameSize = frame_size;
            this.Parent = parent;
        }

        public static PathStep Default(int frame_size, PathStep parent = null)
        {
            return new PathStep(frame_size, parent)
            {
                Index1 = 0,
                Index2 = 0,
                Operation = 0
            };
        }

        public override string ToString()
        {
            return String.Format("F:{0} I1:{1} I2:{2} O{3}", this.FrameSize, this.Index1, this.Index2, this.Operation);
        }

        public void Increment()
        {
            if (++this.Operation >= op_count)
            {
                this.Operation = 0;

                if (++Index2 > this.FrameSize - 2)
                {
                    this.Index2 = 0;
                    if (++this.Index1 > this.FrameSize - 1)
                    {
                        this.Index1 = 0;
                        if (this.Parent != null)
                            this.Parent.Increment();
                    }
                }
            }
        }

        public void Decrement()
        {
            if (--this.Operation < 0)
            {
                this.Operation = op_count - 1;

                if (--this.Index2 < 0)
                {
                    this.Index2 = this.FrameSize - 2;

                    if (--this.Index1 < 0)
                    {
                        this.Index1 = this.FrameSize - 1;

                        if (this.Parent != null)
                            this.Parent.Decrement();
                    }
                }
            }
        }

        public bool IsAtMax()
        {
            return this.Index1 >= this.FrameSize - 1 && this.Index2 >= this.FrameSize - 2 && this.Operation >= op_count - 1;
        }
    }

    public class PathGrid
    {
        public int InitialFrameSize { get; private set; }

        private PathStep[] frames;

        public PathGrid(int initial_frame_size)
        {
            this.InitialFrameSize = initial_frame_size;
            this.add_steps();
        }

        private void add_steps()
        {
            var frms = this.InitialFrameSize;

            this.frames = new PathStep[frms - 1];

            var last = (PathStep)null;

            for (var i = 0; i < this.frames.Length; i++)
            {
                this.frames[i] = PathStep.Default(frms--, last);
                last = this.frames[i];
            }
        }

        public void Increment()
        {
            this.frames[this.frames.Length - 1].Increment();
        }

        public IEnumerable<IPathStep> Steps()
        {
            var maxed = false;

            while (true)
            {
                for (var i = 0; i < this.frames.Length; i++)
                {
                    yield return this.frames[i];
                }

                if (maxed)
                    break;

                this.Increment();

                if (this.frames[0].Index1 >= this.frames[0].FrameSize - 1)
                {
                    if (this.frames[0].IsAtMax())
                    {
                        if (this.frames.All(x => x.IsAtMax()))
                            maxed = true;
                    }
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("{0,-4}|{1,-4}|{2,-4}|{3,-4}\n", "n=?", "1st", "2nd", "Op");

            for (var i = 0; i < this.frames.Length; i++)
                sb.AppendFormat("{0,-4}|{1,-4}|{2,-4}|{3,-4}\n", this.frames[i].FrameSize, this.frames[i].Index1, this.frames[i].Index2, this.frames[i].Operation);

            return sb.ToString();
        }


    }

    public class Path
    {
        public int InitialSize { get; private set; }


        public Path(int initial_frame_size)
        {
            this.InitialSize = initial_frame_size;
        }

        public IEnumerable<Frame> AllFrames()
        {
            for (var i = this.InitialSize; i >= Frame.MinimumSize; i--)
                yield return new Frame(i);
        }
    }

    public class Frame
    {
        public const int MinimumSize = 1;

        public int Size { get; private set; }

        public Frame(int size)
        {
            this.Size = size;
        }

        public IEnumerable<PathStep> AllPossibleSteps()
        {
            return Frame.steps_in_frame(this.Size);
        }

        private static IEnumerable<PathStep> steps_in_frame(int frame_size)
        {
            int op_count = Operation.Operations().Count();

            for (var first = 0; first < frame_size; first++)
            {
                for (var second = 0; second < frame_size - 1; second++)
                {
                    for (var i = 0; i < op_count; i++)
                    {
                        yield return new PathStep(frame_size) { Index1 = first, Index2 = second, Operation = i };
                    }
                }
            }
        }
    }
}
