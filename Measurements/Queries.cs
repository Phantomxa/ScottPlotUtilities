using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Measurements
{
    internal class Queries
    {

        public string CoatingGeometry(string preform_no)
        {
            string query = $"""
                SELECT fiber_id, f.core_id, f.master_preform_no, f.drawn_id, f.parent_id, r.length, draw_start_m, 
                	draw_end_m, send_area, r.status, 
                	ose = 
                		case 
                			when f.orientation = 'O' 
                			then draw_start_m 
                			else draw_end_m 
                		end, 
                	ise = 
                		case 
                			when f.orientation = 'O' 
                			then draw_end_m 
                			else draw_start_m 
                		end, 
                	disp_date, disp_time, d.mach_no
                INTO #fiber_ids
                FROM dsdb..fgm_fibers f, dsdb..rew_fiber r, dsdb..drw_dispense d
                WHERE master_preform_no  = '{preform_no}' -- 'SH7619'  
                and master_preform_no = d.preform_no
                and f.fiber_id = r.new_id

                SELECT t.fiber_id, cgo_mdflag = mdflag, cgo_tc = testcode, SOD_o = coatoutdiam, POD_o = coatindiam, 
                OCE_o = oce, PCE_o = pce, ovality_o = ovality
                INTO #CGO
                FROM  dsdb..wcm_coatose o
                LEFT JOIN
                	(SELECT f.fiber_id, ots = max(timestamp) 
                    FROM #fiber_ids f
                    LEFT JOIN  dsdb..wcm_coatose o ON f.fiber_id = o.fiber_id
                    group by f.fiber_id) t ON t.fiber_id = o.fiber_id
                WHERE t.ots = o.timestamp

                SELECT t.fiber_id, cgi_mdflag = mdflag, cgi_tc = testcode, SOD_i = coatoutdiam, POD_i = coatindiam, 
                OCE_i = oce, PCE_i = pce, ovality_i = ovality
                INTO #CGI
                FROM  dsdb..wcm_coatise o
                LEFT JOIN
                	(SELECT f.fiber_id, ots = max(timestamp) 
                    FROM #fiber_ids f 
                    LEFT JOIN dsdb..wcm_coatise o ON f.fiber_id = o.fiber_id 
                    group by f.fiber_id) t ON t.fiber_id = o.fiber_id
                WHERE t.ots = o.timestamp

                SELECT 'Draw Odometer' = odom, 'Secondary Outer Diameter' = SOD, fiber_id, 'Preform Number' = master_preform_no + ', ' + disp_date, disp_date, mend, send_area, mach_no 
                FROM (SELECT odom = convert(float, ose)/1000.0, SOD = SOD_o, f.fiber_id, master_preform_no, mend = 'O', disp_date, disp_time, send_area, f.mach_no
                        FROM #fiber_ids f, #CGO o WHERE f.fiber_id = o.fiber_id and not isnull(convert(float, SOD_o), 0) = 0.0 and cgo_mdflag = 'M' Union
                        SELECT odom = convert(float, ise)/1000.0, SOD = SOD_i, f.fiber_id, master_preform_no, mend = 'I', disp_date, disp_time, send_area, f.mach_no
                        FROM #fiber_ids f, #CGI i WHERE f.fiber_id = i.fiber_id and not isnull(convert(float, SOD_i), 0) = 0.0 and cgi_mdflag = 'M') a 
                order by odom

                SELECT 'Draw Odometer' = odom, 'Primary Outer Diameter' = POD, fiber_id, 'Preform Number' = master_preform_no + ', ' + disp_date, disp_date, mend, send_area, mach_no 
                FROM (SELECT odom = convert(float, ose)/1000.0, POD = POD_o, f.fiber_id, master_preform_no, mend = 'O', disp_date, disp_time, send_area, f.mach_no
                        FROM #fiber_ids f, #CGO o WHERE f.fiber_id = o.fiber_id and not isnull(convert(float, POD_o), 0) = 0.0 and cgo_mdflag = 'M' Union
                        SELECT odom = convert(float, ise)/1000.0, POD = POD_i, f.fiber_id, master_preform_no, mend = 'I', disp_date, disp_time, send_area, f.mach_no
                        FROM #fiber_ids f, #CGI i WHERE f.fiber_id = i.fiber_id and not isnull(convert(float, POD_i), 0) = 0.0 and cgi_mdflag = 'M') a 
                order by odom

                SELECT 'Draw Odometer' = odom, 'Outer Concentricity Error' = OCE, fiber_id, 'Preform Number' = master_preform_no + ', ' + disp_date, disp_date, mend, send_area, mach_no 
                FROM (SELECT odom = convert(float, ose)/1000.0, OCE = OCE_o, f.fiber_id, master_preform_no, mend = 'O', disp_date, disp_time, send_area, f.mach_no
                        FROM #fiber_ids f, #CGO o WHERE f.fiber_id = o.fiber_id and not isnull(convert(float, OCE_o), 0) = 0.0 and cgo_mdflag = 'M' Union
                        SELECT odom = convert(float, ise)/1000.0, OCE = OCE_i, f.fiber_id, master_preform_no, mend = 'I', disp_date, disp_time, send_area, f.mach_no
                        FROM #fiber_ids f, #CGI i WHERE f.fiber_id = i.fiber_id and not isnull(convert(float, OCE_i), 0) = 0.0 and cgi_mdflag = 'M') a 
                order by odom

                SELECT 'Draw Odometer' = odom, 'Primary Concentricity Error' = PCE, fiber_id, 'Preform Number' = master_preform_no + ', ' + disp_date, disp_date, mend, send_area, mach_no 
                FROM (SELECT odom = convert(float, ose)/1000.0, PCE = PCE_o, f.fiber_id, master_preform_no, mend = 'O', disp_date, disp_time, send_area, f.mach_no
                    FROM #fiber_ids f, #CGO o WHERE f.fiber_id = o.fiber_id and not isnull(convert(float, PCE_o), 0) = 0.0 and cgo_mdflag = 'M' Union
                    SELECT odom = convert(float, ise)/1000.0, PCE = PCE_i, f.fiber_id, master_preform_no, mend = 'I', disp_date, disp_time, send_area, f.mach_no
                    FROM #fiber_ids f, #CGI i WHERE f.fiber_id = i.fiber_id and not isnull(convert(float, PCE_i), 0) = 0.0 and cgi_mdflag = 'M') a 
                order by odom

                SELECT 'Draw Odometer' = odom, 'Ovality' = ovality, fiber_id, 'Preform Number' = master_preform_no + ', ' + disp_date, disp_date, mend, send_area, mach_no 
                FROM (SELECT odom = convert(float, ose)/1000.0, ovality = ovality_o, f.fiber_id, master_preform_no, mend = 'O', disp_date, disp_time, send_area, f.mach_no
                    FROM #fiber_ids f, #CGO o WHERE f.fiber_id = o.fiber_id and not isnull(convert(float, ovality_o), 0) = 0.0 and cgo_mdflag = 'M' Union
                    SELECT odom = convert(float, ise)/1000.0, ovality = ovality_i, f.fiber_id, master_preform_no, mend = 'I', disp_date, disp_time, send_area, f.mach_no
                    FROM #fiber_ids f, #CGI i WHERE f.fiber_id = i.fiber_id and not isnull(convert(float, ovality_i), 0) = 0.0 and cgi_mdflag = 'M') a 
                order by odom

                DROP table #CGO
                DROP table #CGI
                DROP table #fiber_ids
                """;
            return query;
        }

    }
}
