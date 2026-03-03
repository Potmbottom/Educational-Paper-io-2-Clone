using System;
using UniRx;

namespace PaperClone.Domain
{
    public abstract class ReactiveModel : IDisposable
    {
        protected readonly CompositeDisposable Disposables = new CompositeDisposable();

        public virtual void Dispose()
        {
            Disposables.Dispose();
        }
    }
}