using System;

namespace ONE.Utilities
{ 
    public class QualifierItem:IComparable<QualifierItem>
    {
        public string StringValue { get; set; }
        public double Value { get; set; }
        public string Qualifier { get; set; }

        /*
        When determining compliance with an average monthly limit and more than one sample result is available in a month, 
        the discharger shall compute the arithmetic mean unless the data set contains one or more reported determinations 
        of detected but not quantified (DNQ) or not detected (ND). In those cases, the discharger shall compute the median 
        in place of the arithmetic mean in accordance with the following procedure:

        (1) The data set shall be ranked from low to high, 
            reported ND determinations lowest, 
            DNQ determinations next, followed by quantified values (if any). 
            The order of the individual ND or DNQ determinations is unimportant.

        (2) The median value of the data set shall be determined. 
            If the data set has an odd number of data points, then the median is the middle value. 

            If the data set has an even number of data points, 
            then the median is the average of the two values around the middle unless 
            one or both of the points are ND or DNQ, in which case the median value shall 
            be the lower of the two data points where DNQ is lower than a value and ND is lower than DNQ. 
            If a sample result, or the arithmetic mean or median of multiple sample results, is below the reported ML, 
            and there is evidence that the priority pollutant is present in the effluent above an effluent limitation 
            and the discharger conducts a pollutant minimization program (PMP)16 (as described in Section I.7.), 
            the discharger shall not be deemed out of compliance.
         */



        public int CompareTo(QualifierItem other)
        {
            if (other.Qualifier == this.Qualifier)
                return this.Value.CompareTo(other.Value);
            if (other.Qualifier == "ND")
                return 1;
            if (this.Qualifier == "ND")
                return -1;

            if (other.Qualifier == "<")
                return 1;
            if (this.Qualifier == "<")
                return -1;

            if (other.Qualifier == "E")
                return 1;
            if (this.Qualifier == "E")
                return -1;

            if (other.Qualifier == "DNQ")
                return 1;
            if (this.Qualifier == "DNQ")
                return -1;
            if (this.Value == other.Value)
            {
                
                if (other.Qualifier == "TNTC")
                    return -1;
                if (this.Qualifier == "TNTC")
                    return 1;
                if (other.Qualifier == ">")
                    return -1;
                if (this.Qualifier == ">")
                    return 1;
            }
            return this.Value.CompareTo(other.Value);
        }
    }
}
