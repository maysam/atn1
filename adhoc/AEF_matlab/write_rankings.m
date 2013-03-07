function [] = Write_rankings(fout, N, EF, IF, pn)

fid = fopen(fout,'wt');
str = sprintf('%s\t%s',' V   ','EF              ', 'IF');   
fprintf(fid, '%s\n', str);
for i=1:pn       
    str = sprintf('"%s"\t%12.10f \t%12.0f', char(N(i)), EF(i), IF(i));
    fprintf(fid, '%s\n', str);
end
fclose(fid);





