using DumbML;


namespace Astroids {
    public class UnitPolicy {
        public Operation op;
        public float score;
        public int selectionCount;
    }
}
/* Steps
 * Generate population of policies
 * 
 * Evaluate policies using an AC coumpound model
 *   - First train
 *   - Then eval => avg score across multiple games
 *   
 * Next generation of policies
 */

