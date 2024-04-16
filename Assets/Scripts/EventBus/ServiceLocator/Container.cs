
    public static class Container
    {
        public static void Initialize()
        {
            ServiceLocator.Instance.Register(new EventBus());
        }
    }
