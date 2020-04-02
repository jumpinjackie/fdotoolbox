namespace FdoToolbox.Tasks.Controls.BulkCopy
{
    public class CopyTaskDef
    {
        public string SourceConnectionName { get; set; }
        public string SourceSchema { get; set; }
        public string SourceClass { get; set; }
        public string TargetConnectionName { get; set; }
        public string TargetSchema { get; set; }
        public string TargetClass { get; set; }
        public string TaskName { get; set; }
        public bool CreateIfNotExist { get; set; }

        public string EffectiveTargetClass
        {
            get
            {
                if (CreateIfNotExist)
                    return $"{TargetSchema}:{SourceClass}";
                else
                    return $"{TargetSchema}:{TargetClass}";
            }
        }
    }
}
