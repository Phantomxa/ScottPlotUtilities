
namespace UT2_LISG_Stats
{
    internal class Queries
    {

        public static string SybaseCladDeviation(string tower, string startDate, string endDate)
        {
            string syb_QueryClad = $"""
                                    SELECT event_ts, preform_no, CONVERT(INT, mach_no) AS mach_no, CONVERT(FLOAT, datum1) AS clad, CONVERT(FLOAT, datum4) AS clad_dev, CONVERT(INT, length_odom) AS length_odom, CONVERT(INT, feed_pos) AS feed_pos
                                    FROM dsdb..draw_event NOHOLDLOCK 
                                    WHERE  event_ts >= '{startDate}'
                                    AND event_ts <= '{endDate}'
                                    AND event_num = 30
                                    AND mach_no = '{tower}'
                                    order by event_ts asc
                                    """;
            return syb_QueryClad;
        }

        public static string SybaseCladExcursion(string tower, string startDate, string endDate)
        {
            string syb_QueryCladAirlineExcursion =
                                    $"""
                                    SELECT event_ts, length_odom, event_num, mach_no, preform_no, CONVERT(FLOAT, datum1) AS datum1, datum3
                                    FROM dsdb..draw_event NOHOLDLOCK
                                    WHERE
                                    event_num in (32) AND
                                    event_ts >= '{startDate}'
                                    AND event_ts <= '{endDate}'
                                    AND mach_no = '{tower}'
                                    AND datum3 = '1'
                                    order by event_ts asc
                                    """;
            return syb_QueryCladAirlineExcursion;
        }
        public static string SybaseAirlines(string tower, string startDate, string endDate)
        {
            string syb_QueryCladAirlineExcursion =
                                    $"""
                                    SELECT event_ts, length_odom, event_num, mach_no, preform_no, datum3
                                    FROM dsdb..draw_event NOHOLDLOCK
                                    WHERE
                                    event_num in (39) AND
                                    event_ts >= '{startDate}'
                                    AND event_ts <= '{endDate}'
                                    AND mach_no = '{tower}'
                                    AND datum3 = '1'
                                    order by event_ts asc
                                    """;
            return syb_QueryCladAirlineExcursion;
        }
        public static string SybaseFurnacePressures(string tower, string startDate, string endDate)
        {
            string syb_Query_NIF_Pressures = $"""
                                    SELECT event_ts, preform_no, CONVERT(FLOAT, datum1) AS BorePressure, CONVERT(FLOAT, datum3) AS BodyPressure, CONVERT(INT, length_odom) AS length_odom, CONVERT(FLOAT, datum5) AS Temperature
                                    FROM dsdb..draw_event NOHOLDLOCK
                                    WHERE
                                    event_num = 116
                                    AND event_ts >= '{startDate}'
                                    AND event_ts <= '{endDate}'
                                    AND mach_no = '{tower}'
                                    order by event_ts asc
                                    """;
            return syb_Query_NIF_Pressures;
        }
        public static string SybaseFurnaceFlows(string tower, string startDate, string endDate)
        {
            string syb_Query_NIF_Ar = $"""
                                    SELECT event_ts, preform_no, CONVERT(INT, mach_no) AS mach_no, CONVERT(FLOAT, datum1) AS BorePressure, CONVERT(FLOAT, datum2) AS Ar_Bore, CONVERT(FLOAT, datum3) AS Ar_Seal, CONVERT(INT, length_odom) AS length_odom, CONVERT(FLOAT, datum5) as LineSpeed
                                    FROM dsdb..draw_event NOHOLDLOCK
                                    WHERE
                                    event_num = 119
                                    AND event_ts >= '{startDate}'
                                    AND event_ts <= '{endDate}'
                                    AND mach_no = '{tower}'
                                    order by event_ts asc
                                    """;
            return syb_Query_NIF_Ar;
        }
        public static void MyMethod(string preform, string tower, string startDate, string endDate)
        {
            string syb_QueryClad = $@"SELECT event_ts, preform_no, CONVERT(INT, mach_no) AS mach_no, CONVERT(FLOAT, datum1) AS clad, CONVERT(FLOAT, datum4) AS clad_dev, CONVERT(INT, length_odom) AS length_odom
                                    FROM dsdb..draw_event NOHOLDLOCK 
                                    WHERE  
                                    preform_no in ({preform}) AND
                                    event_num = 30
                                    order by event_ts asc";
            string syb_Query_NIF_Pressures = $@"SELECT event_ts, preform_no, CONVERT(INT, mach_no) AS mach_no, CONVERT(FLOAT, datum1) AS BorePressure, CONVERT(FLOAT, datum3) AS BodyPressure, CONVERT(INT, length_odom) AS length_odom, CONVERT(FLOAT, datum5) AS Temperature
                                    FROM dsdb..draw_event NOHOLDLOCK 
                                    WHERE  
                                    preform_no in ({preform}) AND
                                    event_num = 116
                                    order by event_ts asc";
            string syb_Query_NIF_Ar = $@"SELECT event_ts, preform_no, CONVERT(INT, mach_no) AS mach_no, CONVERT(FLOAT, datum1) AS BorePressure, CONVERT(FLOAT, datum2) AS Ar_Bore, CONVERT(FLOAT, datum3) AS Ar_Seal, CONVERT(INT, length_odom) AS length_odom, CONVERT(FLOAT, datum5) as LineSpeed
                                    FROM dsdb..draw_event NOHOLDLOCK 
                                    WHERE  
                                    preform_no in ({preform}) AND
                                    event_num = 119
                                    order by event_ts asc";
            string syb_Query_LS = $@"SELECT event_ts, preform_no, CONVERT(INT, mach_no) AS mach_no, CONVERT(FLOAT, datum5) AS LineSpeed, CONVERT(INT, length_odom) AS length_odom
                                    FROM dsdb..draw_event NOHOLDLOCK 
                                    WHERE  
                                    preform_no in ({preform}) AND
                                    event_num = 130
                                    order by event_ts asc";
            string syb_QueryCladAirlineExcursion =
                                    $@"SELECT length_odom, event_num, mach_no, preform_no, datum1, datum3
                                    FROM dsdb..draw_event NOHOLDLOCK
                                    WHERE  event_num in (32, 39) AND
                                    preform_no in ({preform})
                                    order by event_ts asc";
            string syb_AFC = $@"SELECT event_ts, datum3
                                    FROM dsdb..draw_event NOHOLDLOCK 
                                    WHERE  mach_no = '{tower}' AND
                                    event_num in (130) AND 
                                    event_ts >= '{startDate}'AND
                                    event_ts <= '{endDate}'
                                    order by event_ts asc";
        }


    }
}
