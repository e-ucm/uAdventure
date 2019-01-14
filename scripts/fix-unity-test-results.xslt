<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" version="1.0" encoding="utf-8" indent="yes" cdata-section-elements="message"/>
  <xsl:strip-space elements="*"/>

  <xsl:template match="/">
    <test-run id="2" engine-version="3.0" clr-version="4.0">
      <xsl:copy-of select="(test-suite/@*)[contains('|testcasecount|result|start-time|end-time|duration|total|passed|failed|inconclusive|skipped|asserts|', concat('|', name(), '|'))]" />
      <xsl:apply-templates select="@*|node()"/>
    </test-run>
  </xsl:template>

  <xsl:template match="node()|@*">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
