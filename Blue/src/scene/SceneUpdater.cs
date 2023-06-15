using System.Collections.Generic;

namespace BlueFw;

internal class SceneUpdater : SceneHandler<IUpdatable> {

    static readonly IComparer<IUpdatable> updatableComparer = new UpdatableOrderComparer();
    protected override IComparer<IUpdatable> itemComparer => updatableComparer;

    public void Update() {
        PrepareItemsForHandling();

        for (int i = 0; i < items.Length; i++) {
            if (items.Buffer[i].Active) {
                items.Buffer[i].Update();
            }
        }
    }
}