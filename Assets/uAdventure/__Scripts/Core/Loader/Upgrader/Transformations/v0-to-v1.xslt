<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

    <xsl:output method="xml"/>

    <xsl:template match="grab[not(effect)]">
        <xsl:copy>
            <xsl:apply-templates select="node()|@*"/>
            <effect>
                <remove-element idTarget='{../../@id}'/>
                <condition/>
                <generate-object idTarget='{../../@id}'/>
                <condition/>
            </effect>
        </xsl:copy>
    </xsl:template> 

    <xsl:template match="grab/effect">
        <xsl:copy>
            <xsl:apply-templates select="node()|@*"/>
            <remove-element idTarget='{../../../@id}'/>
            <condition/>
            <generate-object idTarget='{../../../@id}'/>
            <condition/>
        </xsl:copy>
    </xsl:template>

    <xsl:template match="node()|@*">
        <xsl:copy>
            <xsl:apply-templates select="node()|@*"/>
        </xsl:copy>
    </xsl:template>

</xsl:stylesheet>