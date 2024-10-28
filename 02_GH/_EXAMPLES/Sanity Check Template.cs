 //Sanity check examples
 
 //Sanity
        //Checks if Data is null/void
        if (INPUT==null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input parameter Points failed to collect data");
            return;
        }

        
        
        


        //Checks by count
        int pt_count=Points.Count();

        if (INPUT==0)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter Point failed to collect data");
            return;
        }

        //Message box below example
        int Jarvis_count=Jarvis_pts.Count();
        this.Component.Message=Jarvis_count.ToString() + " Point(s)";


        //DEFAULT VALUE

        //Use to refernce string if not list
        if (string.IsNullOrEmpty(Pattern))Pattern=defaultPattern;

        //Use this if in a list
        if (Pattern ==null) Pattern=defaultPattern;

        //Drop down menu from component using pManager
        Grasshopper.Kernel.Parameters.Param_Integer param = pManager[1] as Grasshopper.Kernel.Parameters.Param_Integer;
        

        //sanity check for vectors
        bool vA=Vector_A.IsZero;
        bool vB=Vector_B.IsZero;

        Console.WriteLine(vA.ToString());

        if (vA==true && vB==true)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,"Input failed to collect Vector A and Vector B parameter.");
            return;
        }

        if (vA==true)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,"Input failed to collect Vector A or is zero length vector.");
            return;
        }

        if(vB==true)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,"Input failed to collect Vector B is zero length vector.");
            return;
        }