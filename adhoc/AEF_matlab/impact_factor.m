function [IF] = impactFact(mLinks, ln)
    IF = zeros(ln, 1);
    for i=1:ln
        IF(mLinks(i,2)) = IF(mLinks(i,2)) + 1;
    end
end


