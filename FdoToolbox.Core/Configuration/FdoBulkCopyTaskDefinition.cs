namespace FdoToolbox.Core.Configuration
{
    partial class FdoBulkCopyTaskDefinition
    {
        /// <summary>
        /// Determines whether [is connection defined] [the specified name].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if [is connection defined] [the specified name]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsConnectionDefined(string name)
        {
            foreach (FdoConnectionEntryElement el in this.Connections)
            {
                if (el.name == name)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Symbolically updates all named references to a connection with the new 
        /// connection name.
        /// </summary>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        public void UpdateConnectionReferences(string oldName, string newName)
        {
            foreach (FdoConnectionEntryElement el in this.Connections)
            {
                if (el.name == oldName)
                    el.name = newName;
            }

            foreach (FdoCopyTaskElement el in this.CopyTasks)
            {
                if (el.Source.connection == oldName)
                    el.Source.connection = newName;

                if (el.Target.connection == oldName)
                    el.Target.connection = newName;
            }
        }
    }
}
