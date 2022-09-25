using Core.AsyncTask;
using UnityEngine;
using Zenject;

namespace Application.Init
{
    public interface ISceneInstaller
    {
        IAsyncTask Install(DiContainer container);
        void Uninstall();

        Camera GetCamera();
    }
}