using Lbookshelf.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lbookshelf.Utils
{
    public abstract class IdGenerator
    {
        public abstract void Reset();

        public abstract int Next();

        private static IdGenerator _global = new GlobalIdGenerator();
        public static IdGenerator Global
        {
            get { return _global; }
        }

        private static IdGenerator _local = new LocalIdGenerator();
        public static IdGenerator Local
        {
            get { return _local; }
        }

        private class GlobalIdGenerator : IdGenerator
        {
            public override void Reset()
            {
                throw new NotSupportedException();
            }

            public override int Next()
            {
                if (_currentId == 0)
                {
                    var existing = BookManager.Instance.Books.Select(b => b.Id);
                    if (existing.Count() == 0)
                    {
                        _currentId = 1;
                    }
                    else
                    {
                        _currentId = existing.Max() + 1;
                    }
                }
                else
                {
                    _currentId += 1;
                }

                return _currentId;
            }

            private int _currentId;
        }

        private class LocalIdGenerator : IdGenerator
        {
            public override void Reset()
            {
                _currentId = -1;
            }

            public override int Next()
            {
                _currentId -= 1;

                return _currentId;
            }

            private int _currentId;
        }
    }
}
