using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * This interface must be implemented both by AdaptationProfile and
     * AdaptationRule
     * 
     * @author Javier
     * 
     */

    public interface ContainsAdaptedState
    {

        AdaptedState getAdaptedState();

        void setAdaptedState(AdaptedState adaptedState);


    }
}