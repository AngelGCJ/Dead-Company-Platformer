namespace Events
{
    public delegate void EventHandlerDelegate();

    public delegate void EventHandlerDelegate<in TEvent>(TEvent eventData);

    //START OF EXPERIMENT:
    public delegate void EventHandlerDelegate<in TEvent, in UEvent>(TEvent eventData, UEvent aditionalData);
    //END OF EXPERIMENT

}
