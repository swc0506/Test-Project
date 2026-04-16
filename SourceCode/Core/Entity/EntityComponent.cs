namespace Core.Entity
{
    public abstract class EntityComponent
    {
        protected Entity owner;
        private bool isEnable;

        public bool IsEnable
        {
            get { return isEnable; }
            set
            {
                if (isEnable != value)
                {
                    isEnable = value;
                    if (owner.IsEnable)
                    {
                        if (isEnable)
                        {
                            OnEnable();
                        }
                        else
                        {
                            OnDisable();
                        }
                    }
                }
            }
        }

        internal void Initial(Entity owner)
        {
            this.owner = owner;
            OnInitial();
        }

        internal void Start()
        {
            OnStart();
            IsEnable = true;
        }

        internal void OwnerChangeEnable()
        {
            if (isEnable)
            {
                if (owner.IsEnable)
                {
                    OnEnable();
                }
                else
                {
                    OnDisable();
                }
            }
        }

        internal void Release()
        {
            IsEnable = false;
            OnRelease();
        }

        internal void Dispose()
        {
            OnDispose();
        }


        protected virtual void OnInitial()
        {
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnEnable()
        {
        }


        protected virtual void OnDisable()
        {
        }

        protected virtual void OnRelease()
        {
        }

        protected virtual void OnDispose()
        {
        }
    }
}